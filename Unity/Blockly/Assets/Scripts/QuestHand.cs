using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blockly {

// needs to be placed on a game object that has an OVRCustomSkeleton component.
public class QuestHand : MonoBehaviour {
  private OVRCustomSkeleton skeleton;

  private Chirality chirality;

  public void Awake() {
    skeleton = GetComponent<OVRCustomSkeleton>();
    if (skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.HandLeft) {
      chirality = Chirality.Left;
    } else if (skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.HandRight) {
      chirality = Chirality.Right;
    }
  }

  // TODO cut down on allocation
  public Pose GetCurrentPose() {
    List<Quaternion> boneRotations = skeleton.CustomBones.Select(
      b => b.localRotation).ToList();

    Pose pose = new Pose();
    pose.name = "";
    pose.wristRotation = boneRotations[0];
    pose.fingers = new List<Finger>();

    // thumb rotations
    Finger thumb = new Finger();
    thumb.segmentRotations = new List<Quaternion>();
    for (int i = 2; i < 6; i++) {
      thumb.segmentRotations.Add(boneRotations[i]);
    }
    // index rotations
    Finger index = new Finger();
    index.segmentRotations = new List<Quaternion>();
    for (int i = 6; i < 9; i++) {
      index.segmentRotations.Add(boneRotations[i]);
    }
    // middle rotations
    Finger middle = new Finger();
    middle.segmentRotations = new List<Quaternion>();
    for (int i = 9; i < 12; i++) {
      middle.segmentRotations.Add(boneRotations[i]);
    }
    // ring rotations
    Finger ring = new Finger();
    ring.segmentRotations = new List<Quaternion>();
    for (int i = 12; i < 15; i++) {
      ring.segmentRotations.Add(boneRotations[i]);
    }
    // pinky rotations
    Finger pinky = new Finger();
    pinky.segmentRotations = new List<Quaternion>();
    for (int i = 15; i < 19; i++) {
      pinky.segmentRotations.Add(boneRotations[i]);
    }

    pose.fingers.Add(thumb);
    pose.fingers.Add(index);
    pose.fingers.Add(middle);
    pose.fingers.Add(ring);
    pose.fingers.Add(pinky);

    return pose;
  }

  public Chirality GetChirality() {
    return chirality;
  }
}

}
