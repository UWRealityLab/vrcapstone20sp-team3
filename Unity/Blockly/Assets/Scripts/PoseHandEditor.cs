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
    Debug.Log($"Pose Hand: {poseHand}");
    Debug.Log($"Pose Hand Skeleton: {poseHand.skeleton}");
    Debug.Log($"Pose Hand Skeleton Bones: {poseHand.skeleton.CustomBones}");
    List<Quaternion> boneRotations = poseHand.skeleton.CustomBones.Select(b => b.rotation).ToList();

    PoseHand.Pose pose = new PoseHand.Pose();
    pose.name = "New Gesture";
    pose.wristRotation = boneRotations[0];
    pose.fingers = new List<PoseHand.Finger>();
    // thumb rotations
    PoseHand.Finger thumb = new PoseHand.Finger();
    thumb.segmentRotations = new List<Quaternion>();
    for (int i = 2; i < 6; i++) {
      thumb.segmentRotations.Add(boneRotations[i]);
    }
    // index rotations
    PoseHand.Finger index = new PoseHand.Finger();
    index.segmentRotations = new List<Quaternion>();
    for (int i = 6; i < 9; i++) {
      index.segmentRotations.Add(boneRotations[i]);
    }
    // middle rotations
    PoseHand.Finger middle = new PoseHand.Finger();
    middle.segmentRotations = new List<Quaternion>();
    for (int i = 9; i < 12; i++) {
      middle.segmentRotations.Add(boneRotations[i]);
    }
    // ring rotations
    PoseHand.Finger ring = new PoseHand.Finger();
    ring.segmentRotations = new List<Quaternion>();
    for (int i = 12; i < 15; i++) {
      ring.segmentRotations.Add(boneRotations[i]);
    }
    // pinky rotations
    PoseHand.Finger pinky = new PoseHand.Finger();
    pinky.segmentRotations = new List<Quaternion>();
    for (int i = 15; i < 19; i++) {
      pinky.segmentRotations.Add(boneRotations[i]);
    }

    pose.fingers.Add(thumb);
    pose.fingers.Add(index);
    pose.fingers.Add(middle);
    pose.fingers.Add(ring);
    pose.fingers.Add(pinky);

    poseHand.poses.Add(pose);

    // }
    // foreach (var transform in poseHand.skeleton.CustomBones) {
    //   Debug.Log($"  {i} pos: {transform.position}");
    //   Debug.Log($"  {i} rot: {transform.rotation}");
    //   i++;
    // }
    // Debug.Log("");
  }
}

}
