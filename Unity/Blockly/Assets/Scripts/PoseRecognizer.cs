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

  private IPlayer player;

  private string currLeftPose;
  private string currRightPose;

  public void Start() {
    player = GetComponent<PlayerManager>().GetPlayer();
  }

  public void Update() {
    string leftPose = Recognize(player.GetCurrLeftPose(), leftPoses);
    string rightPose = Recognize(player.GetCurrRightPose(), rightPoses);
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
