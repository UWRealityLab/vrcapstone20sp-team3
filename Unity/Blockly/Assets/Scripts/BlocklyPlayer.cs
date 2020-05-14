using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRSkeleton;
using OculusSampleFramework;
using Oculus;
using static OVRSkeleton;

namespace Blockly {

public class BlocklyPlayer : MonoBehaviour {
  private HandsManager handsManager;

  private Transform leftIndexTip;
  private Transform rightIndexTip;

  #if UNITY_EDITOR
  // positional velocity
  private Vector3 posV;
  // positional acceleration
  private float posA = 1f;

  // rotational velocity
  private Vector3 rotV;
  // rotational acceleration
  private float rotA = 30f;

    // TODO would be ideal if there was an editor-integrated variable, so we could
    // get less janky focus detection.
    private bool isFocused;
  #else
    // TODO is `#else` required?
  #endif

  public void Awake() {
    #if UNITY_EDITOR
      // lock the cursor. don't worry. you can unlock it (in the Unity editor, at
      // least) by pressing escape.
      Cursor.visible = false;
      Cursor.lockState = CursorLockMode.Locked;
      isFocused = true;
    #else
    #endif
  }

  public void Start() {
    handsManager = HandsManager.Instance;
    // TODO wait until hands manager is initialized before letting anything else happen
    Debug.Log($"hands manager is initialized? {handsManager.IsInitialized()}");

    leftIndexTip = handsManager.LeftHandSkeleton.Bones[(int) BoneId.Hand_IndexTip].Transform;
    Debug.Log($"setting left index tip to: {leftIndexTip}");
    Debug.Assert(handsManager.LeftHandSkeleton.Bones[(int) BoneId.Hand_IndexTip].Id == BoneId.Hand_IndexTip);
    rightIndexTip = handsManager.RightHandSkeleton.Bones[(int) BoneId.Hand_IndexTip].Transform;
    Debug.Log($"setting right index tip to: {leftIndexTip}");
    // IOVRSkeletonDataProvider leftProvider = handsManager.LeftHand;
    // leftIndexTip = leftProvider.GetSkeletonPoseData().Bones[BoneId.Hand_IndexTip].Transform;
    // rightIndexTip = handsManager.RightHand.GetSkeletonPoseData().Bones[BoneId.Hand_IndexTip].Transform;
  }

  #if UNITY_EDITOR
    public void Update() {
      Debug.Log($"hands manager is null? {handsManager == null}");
      Debug.Log($"hands manager is initialized? {handsManager.IsInitialized()}");
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
  #else
  #endif

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
    // return handsManager.LeftHand.GetSkeletonPoseData();
    return default(SkeletonPoseData);
  }

  public SkeletonPoseData GetCurrRightPose() {
    // return handsManager.RightHand.GetSkeletonPoseData();
    return default(SkeletonPoseData);
  }
}

}
