using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blockly {

public class DisplayManager : MonoBehaviour {
  public static DisplayManager Instance;

  public TextMesh LeftGestureDisplay;
  public TextMesh RightGestureDisplay;

  public TextMesh LeftPoseDisplay;
  public TextMesh RightPoseDisplay;

  public TextMesh LeftPoseErrorDisplay;
  public TextMesh RightPoseErrorDisplay;

  public void Awake() {
    Debug.Assert(Instance == null);
    Instance = this;
  }

  public void OnUpdateLeftGesture(string gestureName) {
    LeftGestureDisplay.text = gestureName;
  }

  public void OnUpdateRightGesture(string gestureName) {
    RightGestureDisplay.text = gestureName;
  }

  public void OnUpdateLeftPose(string poseName) {
    LeftPoseDisplay.text = poseName;
  }

  public void OnUpdateRightPose(string poseName) {
    RightPoseDisplay.text = poseName;
  }

  public void SetLeftPoseError(float error) {
    LeftPoseErrorDisplay.text = "Error: " + error.ToString("F2");
  }

  public void SetRightPoseError(float error) {
    RightPoseErrorDisplay.text = "Error: " + error.ToString("F2");
  }
}

}
