using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Blockly {

public class PoseRecognizer : MonoBehaviour {
  public StringEvent OnUpdateLeftPose;
  public StringEvent OnUpdateRightPose;

  public float threshold = 43.8f;

  public List<Pose> leftPoses;
  public List<Pose> rightPoses;

  public bool loadPoses;

  private IPlayer player;

  private Pose currLeftPose;
  public Pose CurrLeftPose { get => currLeftPose; }

  private Pose currRightPose;
  public Pose CurrRightPose { get => currRightPose; }

  public void Start() {
    player = GetComponent<PlayerManager>().GetPlayer();

    // Load in saved poses.
    if (loadPoses)
    {
      foreach (string file in Directory.GetFiles(Application.dataPath + "/Poses/left"))
      {
        if (!file.EndsWith("dat"))
        {
          // Ignore any other generated files in this directory.
          continue;
        }
        Debug.Log(file);
        SerializablePose nextSerializablePose;
        using (Stream filestream = File.Open(file, FileMode.Open))
        {
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            nextSerializablePose = (SerializablePose)formatter.Deserialize(filestream);
        }
        Pose nextPose = toPose(nextSerializablePose);
        leftPoses.Add(nextPose);
      }

      foreach (string file in Directory.GetFiles(Application.dataPath + "/Poses/right"))
      {
        if (!file.EndsWith("dat"))
        {
          // Ignore any other generated files in this directory.
          continue;
        }
        Debug.Log(file);
        SerializablePose nextSerializablePose;
        using (Stream filestream = File.Open(file, FileMode.Open))
        {
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            nextSerializablePose = (SerializablePose)formatter.Deserialize(filestream);
        }
        Pose nextPose = toPose(nextSerializablePose);
        rightPoses.Add(nextPose);
      }
    }
  }

  public void Update() {
    Pose leftPose = Recognize(player.GetCurrLeftPose(), leftPoses);
    Pose rightPose = Recognize(player.GetCurrRightPose(), rightPoses);
    if (leftPose.name != currLeftPose.name) {
      OnUpdateLeftPose.Invoke(leftPose.name);
    }
    if (rightPose.name != currRightPose.name) {
      OnUpdateRightPose.Invoke(rightPose.name);
    }
    currLeftPose = leftPose;
    currRightPose = rightPose;
  }

  public Pose Recognize(Pose pose, List<Pose> validPoses) {
    float bestError = Mathf.Infinity;
    Pose bestCandidate = new Pose();
    bestCandidate.name = "No Pose";

    // For each pose
    foreach (var validPose in validPoses) {
      Debug.Assert(pose.fingers.Count == validPose.fingers.Count);
      float error = 0f;
      bool discardPose = false;
      for (int i = 0; i < pose.fingers.Count; i++) {
        Finger finger = pose.fingers[i];
        Finger validFinger = validPose.fingers[i];
        for (int j = 0; j < finger.segmentRotations.Count; j++) {
          float currError = Quaternion.Angle(finger.segmentRotations[j], validFinger.segmentRotations[j]);
          if (currError > threshold) {
            discardPose = true;
            break;
          }
          error += currError;
        }
      }

      if (discardPose) {
        continue;
      }
      if (error < bestError) {
        bestCandidate = validPose;
        bestError = error;
      }
    }
    return bestCandidate;
  }
  
  public void SaveRightPose()
  {
    Pose pose = player.GetCurrRightPose();
    SerializablePose serializablePose = toSerializablePose(pose);
    var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
    using (Stream filestream = File.Open(Application.persistentDataPath + "/gesture_right.dat", FileMode.Create))
    {
        formatter.Serialize(filestream, serializablePose);
    }
  }

  public void SaveLeftPose()
  {
    Pose pose = player.GetCurrLeftPose();
    SerializablePose serializablePose = toSerializablePose(pose);
    var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
    using (Stream filestream = File.Open(Application.persistentDataPath + "/gesture_left.dat", FileMode.Create))
    {
        formatter.Serialize(filestream, serializablePose);
    }
  }

  private SerializablePose toSerializablePose(Pose p)
  {
    List<List<SerializableQuaternion>> serializableFingers = new List<List<SerializableQuaternion>>();
    foreach (Finger finger in p.fingers)
    {
      List<SerializableQuaternion> next = new List<SerializableQuaternion>();
      foreach (Quaternion q in finger.segmentRotations)
      {
        next.Add(new SerializableQuaternion(q.x, q.y, q.z, q.w));
      }
      serializableFingers.Add(next);
    }
    SerializablePose serializablePose = new SerializablePose();
    serializablePose.name = p.name;
    serializablePose.fingers = serializableFingers;
    serializablePose.wristRotation = new SerializableQuaternion(p.wristRotation.x, p.wristRotation.y, p.wristRotation.z, p.wristRotation.w);
    return serializablePose;
  }

  private Pose toPose(SerializablePose sP)
  {
    List<Finger> fingers = new List<Finger>();
    foreach (List<SerializableQuaternion> finger in sP.fingers)
    {
      List<Quaternion> segmentRotations = new List<Quaternion>();
      foreach (SerializableQuaternion rotation in finger)
      {
        segmentRotations.Add(new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w));
      } 
      Finger next = new Finger();
      next.segmentRotations = segmentRotations;
      fingers.Add(next);
    }

    Pose pose = new Pose();
    pose.name = sP.name;
    pose.fingers = fingers;
    pose.wristRotation = new Quaternion(sP.wristRotation.x, sP.wristRotation.y, sP.wristRotation.z, sP.wristRotation.w);
    return pose;
  }
}

[System.Serializable]
public struct SerializableQuaternion
{
  public float x;
  public float y;
  public float z;
  public float w;

  public SerializableQuaternion(float rX, float rY, float rZ, float rW)
  {
    x = rX;
    y = rY;
    z = rZ;
    w = rW;
  }
}

[System.Serializable]
public struct SerializablePose
{
  public string name;
  public List<List<SerializableQuaternion>> fingers;
  public SerializableQuaternion wristRotation;
}

}
