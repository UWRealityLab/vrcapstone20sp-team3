using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Blockly {

public class GestureRecognizer : MonoBehaviour {
  public StringEvent OnRecognizeJointGesture;
  public StringEvent OnRecognizeLeftGesture;
  public StringEvent OnRecognizeRightGesture;

  private PoseRecognizer poseRecognizer;

  private LinkedList<string> recentLeftPoses;
  private LinkedList<string> recentRightPoses;
  // how many of the most recent poses to keep track of
  private static int RECENT_POSE_CAPACITY = 10;

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
    if (recentPoses.Count > RECENT_POSE_CAPACITY) {
      recentPoses.RemoveLast();
    }

    string gesture = RecognizeGesture(recentPoses);
    if (gesture == "CreateModule")
    {
      string gestureOther = leftHand ? recentRightPoses.First.Value : recentLeftPoses.First.Value;
      if (gestureOther == "CreateModule")
      {
        OnRecognizeJointGesture.Invoke("CreateModule");
      }
    } else if (gesture != null) {
      if (leftHand) {
        OnRecognizeLeftGesture.Invoke(gesture);
      } else {
        OnRecognizeRightGesture.Invoke(gesture);
      }
    }
  }

  private string RecognizeGesture(LinkedList<string> recentPoses) {
    if (recentPoses.Count < 2) {
      return null;
    }
    // TODO should be replaced by a simple regex-style matching language
    LinkedListNode<string> curr = recentPoses.First;
    if (curr.Next.Value == "Point") {
      string dir = RecognizeDirection(fingerTrail.GetPositions());
      if (dir == null) {
        if (curr.Value == "Open" && curr.Next.Next != null && curr.Next.Next.Value == "Fist") {
          return "Emit";
        } else {
          return null;
        }
      } else {
        return dir;
      }
    } else if (curr.Value == "Open") {
      curr = curr.Next;
      if (curr.Value == "Fist" || (curr.Value == "Default" && curr.Next.Value == "Fist")) {
        return "Emit";
      }
    } else if (curr.Value == "CreateModule") {
      return "CreateModule";
    }
    return null;
  }

  private string RecognizeDirection(Vector3[] positions) {
    if (CalcPathLength(positions) < 0.11f) {
      Debug.Log("ignoring very short move gesture");
      return null;
    }
    for (int i = 1; i < positions.Length - 1; i++) {
      Vector3 prev = (positions[i] - positions[i-1]).normalized;
      Vector3 curr = (positions[i+1] - positions[i]).normalized;
      float alignment = Vector3.Dot(prev, curr);
      // TODO maybe just compare the path distance vs the path length.
      // ignore high curvature at the beginning and the end (moreso at the
      // beginning), because pose transitions can cause this.
      if (i > 2 && i < positions.Length - 2 && alignment < 0.7) {
        Debug.Log("ignoring loopy move gesture");
        return null;
      }
    }
    Vector3 start = positions[0];
    Vector3 end = positions[positions.Length-1];
    Vector3 dir = (end - start).normalized;
    if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y) && Mathf.Abs(dir.x) > Mathf.Abs(dir.z)) {
      // x component is the largest
      if (dir.x > 0f) {
        return "Right";
      } else {
        return "Left";
      }
    } else if (Mathf.Abs(dir.y) > Mathf.Abs(dir.x) && Mathf.Abs(dir.y) > Mathf.Abs(dir.z)) {
      // y component is the largest
      if (dir.y > 0f) {
        return "Up";
      } else {
        return "Down";
      }
    } else {
      // z component is the largest
      if (dir.z > 0f) {
        return "Forward";
      } else {
        return "Backward";
      }
    }
  }

  private float CalcPathLength(Vector3[] positions) {
    float sum = 0f;
    for (int i = 1; i < positions.Length; i++) {
      sum += Vector3.Distance(positions[i-1], positions[i]);
    }
    return sum;
  }
}

}
