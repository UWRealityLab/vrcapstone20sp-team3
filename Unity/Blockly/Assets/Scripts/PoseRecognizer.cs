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

  public TextMesh leftPoseDisplay;
  public TextMesh rightPoseDisplay;

  public TextMesh gestureDisplay;

  public float threshold = 1f;

  // TODO handle poses for both hands
  private LinkedList<string> recentPoses;
  // how many of the most recent poses to keep track of
  private int numRecordedPoses = 10;

  public void Awake() {
    recentPoses = new LinkedList<string>();
  }

  public void Update() {
    PoseHand.Pose leftPose = Recognize(leftHand.GetCurrentPose(), leftHand.poses);
    PoseHand.Pose rightPose = Recognize(rightHand.GetCurrentPose(), rightHand.poses);
    leftPoseDisplay.text = leftPose.name;
    rightPoseDisplay.text = rightPose.name;

    if (recentPoses.Count == 0 || leftPose.name != recentPoses.First.Value) {
      // user changed their hand pose
      recentPoses.AddFirst(leftPose.name);
      if (recentPoses.Count > numRecordedPoses) {
        recentPoses.RemoveLast();
      }

      string gesture = RecognizeGesture();
      if (gesture == null) {
        gestureDisplay.text = "No Gesture";
      } else {
        gestureDisplay.text = gesture;
      }
    }
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

  public string RecognizeGesture() {
    LinkedListNode<string> curr = recentPoses.First;
    if (curr.Value == "No Pose") {
      curr = curr.Next;
      if (curr != null && curr.Value == "Point") {
        return "Block move";
      }
    } else if (curr.Value == "Open") {
      curr = curr.Next;
      if (curr != null && (curr.Value == "Fist" || (curr.Value == "No Pose" && curr.Next.Value == "Fist"))) {
        return "Emit";
      }
    }
    return null;
  }
}

}
