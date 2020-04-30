using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Blockly {

public class PoseRecognizer : MonoBehaviour {
  public StringEvent OnUpdateLeftPose;
  public StringEvent OnUpdateRightPose;

  public float threshold = 43.8f;

  // TODO should really have the pose hands pulling their data from here, rather
  // than the other way around.
  // public List<Pose> leftPoses;
  // public List<Pose> rightPoses;
  public PoseHand leftHand;
  public PoseHand rightHand;

  private PoseHand.Pose currLeftPose;
  public PoseHand.Pose CurrLeftPose { get => currLeftPose; }

  private PoseHand.Pose currRightPose;
  public PoseHand.Pose CurrRightPose { get => currRightPose; }

  public void Update() {
    PoseHand.Pose leftPose = Recognize(leftHand.GetCurrentPose(), leftHand.poses);
    PoseHand.Pose rightPose = Recognize(rightHand.GetCurrentPose(), rightHand.poses);
    if (leftPose.name != currLeftPose.name) {
      OnUpdateLeftPose.Invoke(leftPose.name);
    }
    if (rightPose.name != currRightPose.name) {
      OnUpdateRightPose.Invoke(rightPose.name);
    }
    currLeftPose = leftPose;
    currRightPose = rightPose;
  }

  public PoseHand.Pose Recognize(PoseHand.Pose pose, List<PoseHand.Pose> validPoses) {
    bool discardPose = false;
    float bestDistSum = Mathf.Infinity;
    PoseHand.Pose bestCandidate = new PoseHand.Pose();
    bestCandidate.name = "No Pose";

    // For each pose
    foreach (var validPose in validPoses) {
      Debug.Assert(pose.fingers.Count == validPose.fingers.Count);
      float distSum = 0f;
      for (int i = 0; i < pose.fingers.Count; i++) {
        PoseHand.Finger finger = pose.fingers[i];
        PoseHand.Finger validFinger = validPose.fingers[i];
        for (int j = 0; j < finger.segmentRotations.Count; j++) {
          if (Quaternion.Angle(finger.segmentRotations[j], validFinger.segmentRotations[j]) > threshold) {
            discardPose = true;
            break;
          }
        }
      }
      if (discardPose) {
        discardPose = false;
        continue;
      }
      if (distSum < bestDistSum) {
        bestCandidate = validPose;
        bestDistSum = distSum;
      }
    }
    return bestCandidate;
  }
}

}
