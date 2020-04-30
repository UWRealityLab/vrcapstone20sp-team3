using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blockly {

public class QuestPlayer : MonoBehaviour, IPlayer {
  public GameObject leftHandObj;
  public GameObject rightHandObj;

  public Transform leftIndexTip;
  public Transform rightIndexTip;

  private QuestHand leftHand;
  private QuestHand rightHand;

  public void Awake() {
    leftHand = leftHandObj.GetComponent<QuestHand>();
    rightHand = rightHandObj.GetComponent<QuestHand>();
  }

  // player interface implementations

  public Transform GetTransform() {
    return transform;
  }

  public Transform GetLeftIndexTipTransform() {
    Debug.Assert(leftHand.GetChirality() == Chirality.Left);
    return leftIndexTip;
  }

  public Transform GetRightIndexTipTransform() {
    Debug.Assert(rightHand.GetChirality() == Chirality.Right);
    return rightIndexTip;
  }

  public Pose GetCurrLeftPose() {
    Debug.Assert(leftHand.GetChirality() == Chirality.Left);
    return leftHand.GetCurrentPose();
  }

  public Pose GetCurrRightPose() {
    Debug.Assert(rightHand.GetChirality() == Chirality.Right);
    return rightHand.GetCurrentPose();
  }
}

}
