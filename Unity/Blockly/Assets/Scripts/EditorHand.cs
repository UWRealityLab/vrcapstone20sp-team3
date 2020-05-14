using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static OVRSkeleton;

namespace Blockly {

// needs to be placed on a game object that has an OVRSkeleton component.
public class EditorHand : MonoBehaviour {
  [Range(50f, 1000f)]
  public float transitionSpeed = 750f;
  [Range(0f, 1f)]
  public float depthAttenuation = 0.6f;

  private OVRHand hand;
  private OVRSkeleton skeleton;

  private List<SkeletonPoseData> poses;
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
    hand = GetComponent<OVRHand>();
    skeleton = GetComponent<OVRSkeleton>();
    PoseRecognizer recognizer = GetComponentInParent<PoseRecognizer>();
    Debug.Assert(recognizer != null);
    if (skeleton.GetSkeletonType() == SkeletonType.HandLeft) {
      poses = recognizer.leftPoses.Select(pose => pose.data).ToList();
      modKey = KeyCode.LeftShift;
    } else if (skeleton.GetSkeletonType() == SkeletonType.HandRight) {
      poses = recognizer.rightPoses.Select(pose => pose.data).ToList();
      modKey = KeyCode.RightShift;
    }
  }

  public void Start() {
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

  public IOVRSkeletonDataProvider GetSkeleton() {
    return hand;
  }

  public Transform GetBoneTransform(BoneId boneId) {
    foreach (var bone in skeleton.Bones) {
      if (bone.Id == boneId) {
        return bone.Transform;
      }
    }
    Debug.Assert(false);
    return null;
  }

  public void ApplyPose(SkeletonPoseData pose) {
    Debug.Assert(false);
    /*
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
    */
  }
}

}
