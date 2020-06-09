using System.Collections.Generic;
using System.IO;
using UnityEngine;
using OculusSampleFramework;
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

  private string currLeftPose;
  private string currRightPose;

  public void Start() {
    Debug.Log("awake pose recognizer");

    // Load in saved poses.
    if (shouldLoadPoses)
    {
      LoadPoses("left");
      LoadPoses("right");
    }
  }

  public void Update() {
    string leftPose = Recognize(BlocklyPlayer.Instance.GetCurrLeftPose(), "left", leftPoses);
    string rightPose = Recognize(BlocklyPlayer.Instance.GetCurrRightPose(), "right", rightPoses);
    if (leftPose != null && leftPose != currLeftPose) {
      Debug.Log($"leftPose: {leftPose}");
      OnUpdateLeftPose.Invoke(leftPose);
      currLeftPose = leftPose;
    }
    if (rightPose != null && rightPose != currRightPose) {
      Debug.Log($"rightPose: {rightPose}");
      OnUpdateRightPose.Invoke(rightPose);
      currRightPose = rightPose;
    }
  }

  // TODO shouldn't need both hand and targetposes
  public string Recognize(SkeletonPoseData pose, string hand, List<Pose> targetPoses) {
    if (!pose.IsDataValid || !pose.IsDataHighConfidence) {
      // Debug.Log("ignoring dodgy input pose");
      if (hand == "left") {
        return currLeftPose;
      } else if (hand == "right") {
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
      if (targetPose.name == "CreateModule") {
        InteractableTool rayTool = null;
        if (hand == "left") {
          rayTool = InteractableToolsCreator.Instance.leftRayTool;
        } else if (hand == "right") {
          rayTool = InteractableToolsCreator.Instance.rightRayTool;
        }
        if (rayTool.ToolInputState != ToolInputState.PrimaryInputDown && rayTool.ToolInputState != ToolInputState.PrimaryInputDownStay) {
          // ignore the create module gesture if a pinch isn't detected by OVR
          continue;
        }
      }

      float error = 0f;
      bool poseDiscarded = false;
      for (int i = 0; i < pose.BoneRotations.Length; i++) {
        if (targetPose.name == "Point" && i >= (int) OVRPlugin.BoneId.Hand_Thumb0 && i <= (int) OVRPlugin.BoneId.Hand_Thumb3) {
          continue;
        }
        OVRPlugin.Quatf inputQuat = pose.BoneRotations[i];
        OVRPlugin.Quatf targetQuat = targetPose.data.BoneRotations[i];
        quatA.Set(inputQuat.x, inputQuat.y, inputQuat.z, inputQuat.w);
        quatB.Set(targetQuat.x, targetQuat.y, targetQuat.z, targetQuat.w);
        error += Quaternion.Angle(quatA, quatB);
        if (error > threshold) {
          poseDiscarded = true;
          break;
        }
      }
      if (!poseDiscarded && error < bestError) {
        // HACK: remapping "Default" -> "Open"
        if (targetPose.name == "Default") {
          bestCandidate = "Open";
        } else {
          bestCandidate = targetPose.name;
        }
        bestError = error;
      }
    }
    if (hand == "left") {
      DisplayManager.Instance.SetLeftPoseError(bestError);
    } else if (hand == "right") {
      DisplayManager.Instance.SetRightPoseError(bestError);
    } else {
      Debug.Assert(false);
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
        formatter.Serialize(filestream, BlocklyPlayer.Instance.GetCurrLeftPose());
      } else if (hand == "right") {
        formatter.Serialize(filestream, BlocklyPlayer.Instance.GetCurrRightPose());
      } else {
        Debug.Assert(false);
      }
    }
  }
}

}
