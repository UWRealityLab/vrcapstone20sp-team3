using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blockly {

public class PoseRecognizer : MonoBehaviour {
  // TODO should really have the pose hands pulling their data from here, rather
  // than the other way around.
  // public List<Pose> leftPoses;
  // public List<Pose> rightPoses;
  public PoseHand leftHand;
  public PoseHand rightHand;

  public TextMesh leftDisplay;
  public TextMesh rightDisplay;

  public float threshold = 1f;

  public void Update() {
    PoseHand.Pose leftPose = Recognize(leftHand.GetCurrentPose(), leftHand.poses);
    PoseHand.Pose rightPose = Recognize(rightHand.GetCurrentPose(), rightHand.poses);
    leftDisplay.text = leftPose.name;
    rightDisplay.text = rightPose.name;
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
