using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRSkeleton;

namespace Blockly {

public class EditorPlayer : MonoBehaviour, IPlayer {
  private Transform leftIndexTip;
  private Transform rightIndexTip;

  private EditorHand leftHand;
  private EditorHand rightHand;

  // positional velocity
  private Vector3 posV;
  // positional acceleration
  private float posA = 5f;

  // rotational velocity
  private Vector3 rotV;
  // rotational acceleration
  private float rotA = 30f;

  // TODO would be ideal if there was an editor-integrated variable, so we could
  // get less janky focus detection.
  private bool isFocused;

  public void Awake() {
    // lock the cursor. don't worry. you can unlock it (in the Unity editor, at
    // least) by pressing escape.
    Cursor.visible = false;
    Cursor.lockState = CursorLockMode.Locked;
    isFocused = true;

    List<EditorHand> hands = new List<EditorHand>();
    GetComponentsInChildren<EditorHand>(hands);
    Debug.Assert(hands.Count == 2);
    if (hands[0].GetSkeleton().GetSkeletonType() == SkeletonType.HandLeft) {
      leftHand = hands[0];
      rightHand = hands[1];
    } else {
      leftHand = hands[1];
      rightHand = hands[0];
    }
    Debug.Assert(leftHand.GetSkeleton().GetSkeletonType() == SkeletonType.HandLeft);
    Debug.Assert(rightHand.GetSkeleton().GetSkeletonType() == SkeletonType.HandRight);
    leftIndexTip = leftHand.GetBoneTransform(BoneId.Hand_IndexTip);
    rightIndexTip = rightHand.GetBoneTransform(BoneId.Hand_IndexTip);
  }

  public void Update() {
    if (Input.GetKeyDown(KeyCode.Escape)) {
      isFocused = !isFocused;
    }
    if (!isFocused) {
      return;
    }

    UpdatePosition();
    UpdateRotation();
  }

  private void UpdatePosition() {
    // forwards
    if (Input.GetKey(KeyCode.W)) {
      posV.z += posA;
    }
    // backwards
    if (Input.GetKey(KeyCode.S)) {
      posV.z -= posA;
    }
    // left
    if (Input.GetKey(KeyCode.A)) {
      posV.x -= posA;
    }
    // right
    if (Input.GetKey(KeyCode.D)) {
      posV.x += posA;
    }
    // up
    if (Input.GetKey(KeyCode.E)) {
      posV.y += posA;
    }
    // down
    if (Input.GetKey(KeyCode.Q)) {
      posV.y -= posA;
    }
    posV *= Time.deltaTime;

    transform.Translate(posV);
    posV *= 0.85f;
  }

  private void UpdateRotation() {
    // we don't multiply by delta time, because large camera movements
    // resulting from lag spikes are very jarring.
    rotV.y += rotA * Input.GetAxis("Mouse X");
    rotV.x -= rotA * Input.GetAxis("Mouse Y");
    rotV.x = Mathf.Clamp(rotV.x, -80f, 80f);

    transform.eulerAngles = rotV;
  }

  // player interface implementations

  public Transform GetTransform() {
    return transform;
  }

  public Transform GetLeftIndexTipTransform() {
    // Debug.Assert(leftHand.GetChirality() == Chirality.Left);
    return leftIndexTip;
  }

  public Transform GetRightIndexTipTransform() {
    // Debug.Assert(rightHand.GetChirality() == Chirality.Right);
    return rightIndexTip;
  }

  public SkeletonPoseData GetCurrLeftPose() {
    // Debug.Assert(leftHand.GetChirality() == Chirality.Left);
    return leftHand.GetSkeleton().GetSkeletonPoseData();
  }

  public SkeletonPoseData GetCurrRightPose() {
    // Debug.Assert(rightHand.GetChirality() == Chirality.Right);
    return rightHand.GetSkeleton().GetSkeletonPoseData();
  }
}

}
