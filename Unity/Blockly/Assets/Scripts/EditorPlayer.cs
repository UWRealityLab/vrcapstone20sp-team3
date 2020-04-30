using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blockly {

public class EditorPlayer : MonoBehaviour {
  // positional velocity
  private Vector3 posV;
  // positional acceleration
  private float posA = 1f;

  // rotational velocity
  private Vector3 rotV;
  // rotational acceleration
  private float rotA = 30f;

  public GameObject thumbTip;
  public GameObject indexTip;
  public GameObject middleTip;
  public GameObject ringTip;
  public GameObject pinkyTip;

  // TODO would be ideal if there was an editor-integrated variable, so we could
  // get less janky focus detection.
  private bool isFocused;

  public void Awake() {
    // lock the cursor. don't worry. you can unlock it (in the Unity editor, at
    // least) by pressing escape.
    Cursor.visible = false;
    Cursor.lockState = CursorLockMode.Locked;
    isFocused = true;
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
}

}
