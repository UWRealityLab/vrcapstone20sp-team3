using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRSkeleton;

namespace Blockly {

public class QuestPlayer : MonoBehaviour, IPlayer {

  public GameObject leftHandObj;
  public GameObject rightHandObj;

  public Transform leftIndexTip;
  public Transform rightIndexTip;

  private IOVRSkeletonDataProvider leftHand;
  private IOVRSkeletonDataProvider rightHand;

  public void Awake() {
    leftHand = leftHandObj.GetComponent<IOVRSkeletonDataProvider>();
    Debug.Assert(leftHand.GetSkeletonType() == SkeletonType.HandLeft);
    rightHand = rightHandObj.GetComponent<IOVRSkeletonDataProvider>();
    Debug.Assert(rightHand.GetSkeletonType() == SkeletonType.HandRight);
  }

  // player interface implementations

  public Transform GetTransform() {
    return transform;
  }

  public Transform GetLeftIndexTipTransform() {
    return leftIndexTip;
  }

  public Transform GetRightIndexTipTransform() {
    return rightIndexTip;
  }

  public SkeletonPoseData GetCurrLeftPose() {
    Debug.Assert(leftHand.GetSkeletonType() == SkeletonType.HandLeft);
    return leftHand.GetSkeletonPoseData();
  }

  public SkeletonPoseData GetCurrRightPose() {
    Debug.Assert(rightHand.GetSkeletonType() == SkeletonType.HandRight);
    return rightHand.GetSkeletonPoseData();
  }
}

}
