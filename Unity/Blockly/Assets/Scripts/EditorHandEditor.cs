using UnityEngine;
using UnityEditor;
using static OVRSkeleton;

#if UNITY_EDITOR

namespace Blockly {

[CustomEditor(typeof(EditorHand))]
public class EditorHandEditor : Editor {
  string poseName;

  public override void OnInspectorGUI() {
    DrawDefaultInspector();
    poseName = EditorGUILayout.TextField("Pose Name", poseName);
    if (GUILayout.Button("Save Pose")) {
      SavePose();
    }
  }

  public void SavePose() {
    poseName = poseName.Trim();
    EditorHand hand = (EditorHand) target;
    SkeletonPoseData pose = hand.GetSkeleton().GetSkeletonPoseData();
    PoseRecognizer poseRecognizer = hand.GetComponentInParent<PoseRecognizer>();
    if (hand.GetSkeleton().GetSkeletonType() == SkeletonType.HandLeft) {
      poseRecognizer.leftPoses.Add(new PoseRecognizer.Pose(poseName, pose));
    } else {
      poseRecognizer.rightPoses.Add(new PoseRecognizer.Pose(poseName, pose));
    }
  }
}

}

#endif
