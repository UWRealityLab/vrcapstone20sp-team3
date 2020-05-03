using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blockly {

// needs to be placed on a game object that has an OVRCustomSkeleton component.
public class EditorHand : MonoBehaviour {
  public OVRCustomSkeleton skeleton;
  [Range(50f, 1000f)]
  public float transitionSpeed = 750f;
  [Range(0f, 1f)]
  public float depthAttenuation = 0.6f;

  private List<Pose> poses;
  private static readonly KeyCode[] poseKeys = {
    KeyCode.Alpha1,
    KeyCode.Alpha2,
    KeyCode.Alpha3,
    KeyCode.Alpha4,
    KeyCode.Alpha5,
    KeyCode.Alpha6,
    KeyCode.Alpha7,
    KeyCode.Alpha8,
    KeyCode.Alpha9,
    KeyCode.Alpha0,
  };
  private KeyCode modKey;

  public void Awake() {
    PoseRecognizer recognizer = GetComponentInParent<PoseRecognizer>();
    if (skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.HandLeft) {
      poses = recognizer.leftPoses;
      modKey = KeyCode.LeftShift;
    } else if (skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.HandRight) {
      poses = recognizer.rightPoses;
      modKey = KeyCode.RightShift;
    }
  }

  public void Update() {
    if (Input.GetKey(modKey)) {
      for (int i = 0; i < poses.Count; i++) {
        if (Input.GetKey(poseKeys[i])) {
          ApplyPose(poses[i]);
          break;
        }
      }
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

  public void ApplyPose(Pose pose) {
    List<Transform> boneTransforms = skeleton.CustomBones;

    // NOTE: eventually, we may want poses that incorporate the wrist rotation
    // (e.g., to disambiguate between a thumbs up and a thumbs down).
    // boneTransforms[0].rotation = pose.wristRotation;

    int offset = 2;
    foreach (var finger in pose.fingers) {
      int depth = 0;
      foreach (var targetRotation in finger.segmentRotations) {
        if (transitionSpeed == 0f) {
          boneTransforms[offset].localRotation = targetRotation;
        } else {
          float speedMultiplier = Mathf.Pow(depthAttenuation, depth);
          boneTransforms[offset].localRotation = Quaternion.RotateTowards(
            boneTransforms[offset].localRotation,
            targetRotation,
            transitionSpeed * speedMultiplier * Time.deltaTime);
        }
        offset++;
        depth++;
      }
    }
  }

  public Chirality GetChirality() {
    if (skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.HandLeft) {
      return Chirality.Left;
    } else if (skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.HandRight) {
      return Chirality.Right;
    } else {
      Debug.Assert(false);
      return Chirality.Right;
    }
  }

  public OVRSkeleton GetSkeleton() {
    return skeleton;
  }
}

}
