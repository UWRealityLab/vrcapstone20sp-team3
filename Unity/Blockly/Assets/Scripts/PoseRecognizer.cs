using System.Collections.Generic;
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
    public SkeletonPoseData data;
    public string name;

    public Pose(SkeletonPoseData data, string name) {
      this.data = data;
      this.name = name;
    }
  }
  public List<Pose> leftPoses;
  public List<Pose> rightPoses;

  private BlocklyPlayer player;

  private string currLeftPose;
  private string currRightPose;

  public void Start() {
    Debug.Log("awake pose recognizer");
    player = GetComponent<BlocklyPlayer>();
  }

  public void Update() {
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
    float bestError = Mathf.Infinity;
    string bestCandidate = null;

    // For each pose
    foreach (var targetPose in targetPoses) {
      // Debug.Assert(pose.fingers.Count == validPose.fingers.Count);
      Debug.Log($"checking against pose {targetPose.name}");
      float error = 0f;
      bool discardPose = false;
      // for (int i = 0; i < pose.fingers.Count; i++) {
      //   Finger finger = pose.fingers[i];
      //   Finger validFinger = validPose.fingers[i];
      //   for (int j = 0; j < finger.segmentRotations.Count; j++) {
      //     float currError = Quaternion.Angle(finger.segmentRotations[j], validFinger.segmentRotations[j]);
      //     if (currError > threshold) {
      //       discardPose = true;
      //       break;
      //     }
      //     error += currError;
      //   }
      // }

      // if (discardPose) {
      //   continue;
      // }
      if (error < bestError) {
        bestCandidate = targetPose.name;
        bestError = error;
      }
    }
    return bestCandidate;
  }
}

}
