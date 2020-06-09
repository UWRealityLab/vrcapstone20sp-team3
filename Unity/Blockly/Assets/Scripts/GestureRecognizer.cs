using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using OculusSampleFramework;

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

  // keep track of how many seconds have elapsed since we've been in
  // "Open"/"Default" and since we've left "Open"/"Default", so we can smoothly
  // turn the ray tool on and off, respectively
  private static float SATURATION_THRESH = 0.3f;
  private static float SATURATION_LIMIT = 0.4f;
  private float leftRaySatCounter = 0f;
  private float rightRaySatCounter = 0f;
  private bool leftEnableState = true;
  private bool rightEnableState = true;

  // for recognizing move gestures
  [SerializeField] [NotNull]
  private FingerTrail leftFingerTrail;
  [SerializeField] [NotNull]
  private FingerTrail rightFingerTrail;

  public void Awake() {
    poseRecognizer = GetComponent<PoseRecognizer>();
    recentLeftPoses = new LinkedList<string>();
    recentRightPoses = new LinkedList<string>();
  }

  public void OnUpdateLeftPose(string poseName) {
    OnUpdatePose(true, poseName, recentLeftPoses);
  }

  public void OnUpdateRightPose(string poseName) {
    OnUpdatePose(false, poseName, recentRightPoses);
  }

  public void Update() {
    UpdateSingle(true, recentLeftPoses);
    UpdateSingle(false, recentRightPoses);
  }

  private void UpdateSingle(bool leftHand, LinkedList<string> recentPoses) {
    if (recentPoses.Count == 0) return;

    RayTool rayTool = null;
    float satCounter = 0f;
    bool enableState = false;
    if (leftHand) {
      rayTool = InteractableToolsCreator.Instance.leftRayTool;
      satCounter = leftRaySatCounter;
      enableState = leftEnableState;
    } else {
      rayTool = InteractableToolsCreator.Instance.rightRayTool;
      satCounter = rightRaySatCounter;
      enableState = rightEnableState;
    }

    bool isOpened = recentPoses.First.Value == "Open" || recentPoses.First.Value == "Default";
    isOpened = isOpened || ((rayTool.ToolInputState == ToolInputState.PrimaryInputDown || rayTool.ToolInputState == ToolInputState.PrimaryInputDownStay) && recentPoses.First.Value != "Fist");

    // we desaturate faster than we saturate
    float delta = 0f;
    if (isOpened) {
      delta = Time.deltaTime;
    } else if (recentPoses.First.Value == "Point") {
      delta = -Time.deltaTime * 2f;
    } else if (recentPoses.First.Value == "CreateModule") {
      delta = -Time.deltaTime * 1.5f;
    } else {
      delta = -Time.deltaTime;
    }
    satCounter = Mathf.Clamp(satCounter + delta, -SATURATION_LIMIT, SATURATION_LIMIT);

    if (satCounter < -SATURATION_THRESH) {
      enableState = false;
    } else if (satCounter > SATURATION_THRESH) {
      enableState = true;
    }

    rayTool.EnableState = enableState;

    if (leftHand) {
      leftRaySatCounter = satCounter;
      leftEnableState = enableState;
      DisplayManager.Instance.SetLeftSat(leftRaySatCounter);
    } else {
      rightRaySatCounter = satCounter;
      rightEnableState = enableState;
      DisplayManager.Instance.SetRightSat(rightRaySatCounter);
    }
  }

  public void OnUpdatePose(
    bool leftHand, string newPose, LinkedList<string> recentPoses) {
    // ensure this pose is new
    Debug.Assert(recentPoses.Count == 0 || newPose != recentPoses.First.Value);
    if (recentPoses.Count > 0 && newPose == recentPoses.First.Value) {
      string handName = leftHand ? "left" : "right";
      Debug.Log("got duplicate pose \"" + newPose + "\" in " + handName + " hand");
    }

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
        leftFingerTrail.ToggleModuleCreation();
        OnRecognizeJointGesture.Invoke("CreateModule");
      }
    } else if (gesture == "Loop") {
      int numLoops = -1;
      int moduleName;
      if (leftHand) {
        moduleName = BlocklyPlayer.Instance.currRightSelectedModule;
        numLoops = leftFingerTrail.GetNumLoopIterations();
      } else {
        moduleName = BlocklyPlayer.Instance.currLeftSelectedModule;
        numLoops = rightFingerTrail.GetNumLoopIterations();
      }
      if (numLoops > 0 && moduleName != -1) {
        for (int i = 0; i < numLoops; i++) {
          ModuleController.Instance.OnUseModule(moduleName);
        }
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
    if (recentPoses.Count < 3) {
      return null;
    }
    // TODO should be replaced by a simple regex-style matching language
    LinkedListNode<string> curr = recentPoses.First;
    if (curr.Next.Value == "Point") {
      string dir = null;
      if (recentPoses == recentLeftPoses) {
        dir = RecognizeDirection(leftFingerTrail.GetPositions());
      } else if (recentPoses == recentRightPoses) {
        dir = RecognizeDirection(rightFingerTrail.GetPositions());
      } else {
        Debug.Assert(false);
      }
      if (dir == null) {
        if (curr.Value == "Open" && curr.Next.Next != null && curr.Next.Next.Value == "Fist") {
          return "Emit";
        } else {
          return "Loop";
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
