using UnityEngine;
using UnityEditor;

namespace Blockly {

[CustomEditor(typeof(EditorHand))]
public class EditorHandEditor : Editor {
  public override void OnInspectorGUI() {
    DrawDefaultInspector();
    if (GUILayout.Button("Save Pose")) {
      SavePose();
    }
  }

  public void SavePose() {
    EditorHand hand = (EditorHand) target;
    Pose pose = hand.GetCurrentPose();
    pose.name = "New Pose";
    PoseRecognizer poseRecognizer = hand.GetComponentInParent<PoseRecognizer>();
    if (hand.GetChirality() == Chirality.Left) {
      poseRecognizer.leftPoses.Add(pose);
    } else {
      poseRecognizer.rightPoses.Add(pose);
    }
  }
}

}
