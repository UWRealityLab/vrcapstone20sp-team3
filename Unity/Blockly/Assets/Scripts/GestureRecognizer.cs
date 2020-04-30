using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Blockly {

public class GestureRecognizer : MonoBehaviour {
  public StringEvent OnRecognizeLeftGesture;
  public StringEvent OnRecognizeRightGesture;

  private PoseRecognizer poseRecognizer;

  private LinkedList<string> recentLeftPoses;
  private LinkedList<string> recentRightPoses;
  // how many of the most recent poses to keep track of
  private int recentPoseCapacity = 10;

  // for recognizing move gestures
  private FingerTrail fingerTrail;

  public void Awake() {
    poseRecognizer = GetComponent<PoseRecognizer>();
    recentLeftPoses = new LinkedList<string>();
    recentRightPoses = new LinkedList<string>();
    fingerTrail = GetComponent<FingerTrail>();
  }

  public void OnUpdateLeftPose(string poseName) {
    OnUpdatePose(true, poseName, recentLeftPoses);
  }

  public void OnUpdateRightPose(string poseName) {
    OnUpdatePose(false, poseName, recentRightPoses);
  }

  public void OnUpdatePose(
    bool leftHand, string newPose, LinkedList<string> recentPoses) {
    // ensure this pose is new
    Debug.Assert(recentPoses.Count == 0 || newPose != recentPoses.First.Value);

    recentPoses.AddFirst(newPose);
    if (recentPoses.Count > recentPoseCapacity) {
      recentPoses.RemoveLast();
    }

    string gesture = RecognizeGesture(recentPoses);
    if (gesture != null) {
      if (leftHand) {
        OnRecognizeLeftGesture.Invoke(gesture);
      } else {
        OnRecognizeRightGesture.Invoke(gesture);
      }
    }
  }

  public string RecognizeGesture(LinkedList<string> recentPoses) {
    // TODO should be replaced by a simple regex-style matching language
    LinkedListNode<string> curr = recentPoses.First;
    if (curr.Value == "No Pose") {
      curr = curr.Next;
      if (curr != null && curr.Value == "Point") {
        return fingerTrail.RecognizeDirection();
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
