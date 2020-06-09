using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blockly {

public class DisplayManager : MonoBehaviour {
  public static DisplayManager Instance;

  [NotNull]
  public TextMesh LeftGestureDisplay;
  [NotNull]
  public TextMesh RightGestureDisplay;
  [NotNull]
  public TextMesh JointGestureDisplay;

  [NotNull]
  public TextMesh LeftPoseDisplay;
  [NotNull]
  public TextMesh RightPoseDisplay;

  [NotNull]
  public TextMesh LeftPoseErrorDisplay;
  [NotNull]
  public TextMesh RightPoseErrorDisplay;

  [NotNull]
  public TextMesh LeftSatDisplay;
  [NotNull]
  public TextMesh RightSatDisplay;

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

  public void OnUpdateJointGesture(string gestureName) {
    JointGestureDisplay.text = gestureName;
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

  public void SetLeftSat(float error) {
    LeftSatDisplay.text = "Sat: " + error.ToString("F2");
  }

  public void SetRightSat(float error) {
    RightSatDisplay.text = "Sat: " + error.ToString("F2");
  }
}

}
