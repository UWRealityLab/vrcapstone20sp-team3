using System.Collections.Generic;
using UnityEngine;

namespace Blockly {

public class PoseRecognizer : MonoBehaviour {
  public StringEvent OnUpdateLeftPose;
  public StringEvent OnUpdateRightPose;

  public float threshold = 43.8f;

  public List<Pose> leftPoses;
  public List<Pose> rightPoses;

  public EditorHand leftHand;
  public EditorHand rightHand;

  private Pose currLeftPose;
  public Pose CurrLeftPose { get => currLeftPose; }

  private Pose currRightPose;
  public Pose CurrRightPose { get => currRightPose; }

  public void Update() {
    Pose leftPose = Recognize(leftHand.GetCurrentPose(), leftPoses);
    Pose rightPose = Recognize(rightHand.GetCurrentPose(), rightPoses);
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
    bool discardPose = false;
    float bestDistSum = Mathf.Infinity;
    Pose bestCandidate = new Pose();
    bestCandidate.name = "No Pose";

    // For each pose
    foreach (var validPose in validPoses) {
      Debug.Assert(pose.fingers.Count == validPose.fingers.Count);
      float distSum = 0f;
      for (int i = 0; i < pose.fingers.Count; i++) {
        Finger finger = pose.fingers[i];
        Finger validFinger = validPose.fingers[i];
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
