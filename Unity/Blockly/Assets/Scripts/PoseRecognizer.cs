using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static OVRSkeleton;

namespace Blockly {

public class PoseRecognizer : MonoBehaviour {
  public StringEvent OnUpdateLeftPose;
  public StringEvent OnUpdateRightPose;

  public float threshold = 43.8f;

  // NOTE: we use a custom struct, because dictionaries aren't integrated in
  // the inspector
  [System.Serializable]
  public struct Pose {
    public string name;
    public SkeletonPoseData data;

    public Pose(string name, SkeletonPoseData data) {
      this.name = name;
      this.data = data;
    }
  }

  public List<Pose> leftPoses;
  public List<Pose> rightPoses;

  public bool shouldLoadPoses;

  private IPlayer player;

  private string currLeftPose;
  private string currRightPose;

  public void Start() {
    Debug.Log("awake pose recognizer");
    player = GetComponent<BlocklyPlayer>();

    // Load in saved poses.
    if (shouldLoadPoses)
    {
      LoadPoses("left");
      LoadPoses("right");
    }
  }

  public void Update() {
    foreach (var targetPose in leftPoses) {
      Debug.Log($"checking pose {targetPose.name}");
      Debug.Log(targetPose.data);
      Debug.Log(targetPose.data.BoneRotations);
    }
    Debug.Log("recognizing left pose");
    string leftPose = Recognize(player.GetCurrLeftPose(), leftPoses);
    Debug.Log($"leftPose: {leftPose}");
    Debug.Log("recognizing right pose");
    string rightPose = Recognize(player.GetCurrRightPose(), rightPoses);
    Debug.Log($"rightPose: {rightPose}");
    if (leftPose != currLeftPose) {
      OnUpdateLeftPose.Invoke(leftPose);
    }
    if (rightPose != currRightPose) {
      OnUpdateRightPose.Invoke(rightPose);
    }
    currLeftPose = leftPose;
    currRightPose = rightPose;
  }

  public string Recognize(SkeletonPoseData pose, List<Pose> targetPoses) {
    if (!pose.IsDataValid || !pose.IsDataHighConfidence) {
      Debug.Log("ignoring dodgy input pose");
      if (targetPoses == leftPoses) {
        return currLeftPose;
      } else if (targetPoses == rightPoses) {
        return currRightPose;
      }
      Debug.Assert(false);
      return null;
    }
    float bestError = Mathf.Infinity;
    string bestCandidate = null;

    // use Unity quats to copy OVR quats into, so we can use `Quaternion.Angle`
    // to calculate error terms
    Quaternion quatA = new Quaternion();
    Quaternion quatB = new Quaternion();
    foreach (var targetPose in targetPoses) {
      Debug.Log($"checking against pose {targetPose.name}");
      float error = 0f;
      for (int i = 0; i < pose.BoneRotations.Length; i++) {
        Debug.Log($"A error={error}");
        OVRPlugin.Quatf inputQuat = pose.BoneRotations[i];
        Debug.Log($"B inputQuat={inputQuat}");
        Debug.Log($"B.5 targetPose.data={targetPose.data}");
        Debug.Log($"B.75 targetPose.data.BoneRotations={targetPose.data.BoneRotations}");
        OVRPlugin.Quatf targetQuat = targetPose.data.BoneRotations[i];
        Debug.Log($"C targetQuat={targetQuat}");
        quatA.Set(inputQuat.x, inputQuat.y, inputQuat.z, inputQuat.w);
        Debug.Log($"D");
        quatB.Set(targetQuat.x, targetQuat.y, targetQuat.z, targetQuat.w);
        Debug.Log($"E");
        error += Quaternion.Angle(quatA, quatB);
        Debug.Log($"F error={error}");
        if (error > threshold) {
          Debug.Log($"  discarding pose (error={error} > threshold={threshold})");
          break;
        }
      }
      if (error < bestError) {
        bestCandidate = targetPose.name;
        bestError = error;
      }
    }
    return bestCandidate;
  }

  private void LoadPoses(string hand) {
    foreach (string file in Directory.GetFiles(Application.dataPath + "/Poses/" + hand))
    {
      if (!file.EndsWith("dat"))
      {
        // Ignore any other generated files in this directory.
        continue;
      }
      string poseName = Path.GetFileNameWithoutExtension(file);
      Debug.Log($"loading {poseName} from {file}");
      SkeletonPoseData poseData;
      using (Stream filestream = File.Open(file, FileMode.Open))
      {
        var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        poseData = (SkeletonPoseData) formatter.Deserialize(filestream);
      }
      Debug.Log($"poseData: {poseData}");
      Debug.Log($"poseData.BoneRotations: {poseData.BoneRotations}");
      Debug.Log($"poseData.BoneRotations[3]: {poseData.BoneRotations[3]}");
      Pose pose = new Pose(poseName, poseData);
      if (hand == "left") {
        leftPoses.Add(pose);
      } else if (hand == "right") {
        rightPoses.Add(pose);
      } else {
        Debug.Assert(false);
      }
    }
  }

  public void SaveLeftPose()
  {
    SavePose("left");
  }

  public void SaveRightPose()
  {
    SavePose("right");
  }

  private void SavePose(string hand)  {
    string filePath = Application.persistentDataPath + $"/pose_{hand}.dat";
    Debug.Log($"saving {hand} pose to {filePath}");
    // SerializablePose serializablePose = toSerializablePose(pose);
    var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
    using (Stream filestream = File.Open(filePath, FileMode.Create))
    {
      if (hand == "left") {
        formatter.Serialize(filestream, player.GetCurrLeftPose());
      } else if (hand == "right") {
        formatter.Serialize(filestream, player.GetCurrRightPose());
      } else {
        Debug.Assert(false);
      }
    }
  }
}

}
