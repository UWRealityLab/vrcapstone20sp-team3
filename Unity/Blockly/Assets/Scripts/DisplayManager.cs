using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blockly {

public class DisplayManager : MonoBehaviour {
  public static DisplayManager Instance;

  public TextMesh leftPoseDisplay;
  public TextMesh rightPoseDisplay;

  public TextMesh leftGestureDisplay;
  public TextMesh rightGestureDisplay;

  public void Awake() {
    Debug.Assert(Instance == null);
    Instance = this;
  }

  public void OnUpdateLeftPose(string poseName) {
    leftPoseDisplay.text = poseName;
  }

  public void OnUpdateRightPose(string poseName) {
    rightPoseDisplay.text = poseName;
  }

  public void OnUpdateLeftGesture(string gestureName) {
    leftGestureDisplay.text = gestureName;
  }

  public void OnUpdateRightGesture(string gestureName) {
    rightGestureDisplay.text = gestureName;
  }
}

}
