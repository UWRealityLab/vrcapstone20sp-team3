using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Blockly {

[CustomEditor(typeof(PoseHand))]
public class PoseHandEditor : Editor {
  public override void OnInspectorGUI() {
    DrawDefaultInspector();
    if (GUILayout.Button("Save Pose")) {
      SavePose();
    }
  }

  public void SavePose() {
    PoseHand poseHand = (PoseHand) target;
    PoseHand.Pose pose = poseHand.GetCurrentPose();
    pose.name = "New Pose";
    poseHand.poses.Add(pose);
  }
}

}
