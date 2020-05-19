using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragModulePlayAreaChecker : MonoBehaviour {
    private BoxCollider collider;
    private BoxCollider validAreaCollider;

    private bool inPlayArea = false;

    public void Awake() {
        collider = GetComponent<BoxCollider>();
        validAreaCollider = GameObject.Find("/Valid Area").GetComponent<BoxCollider>();
    }

    public void OnTriggerEnter(Collider other) {
        if (other == validAreaCollider) {
            inPlayArea = true;
        }
    }

    public void OnTriggerExit(Collider other) {
        if (other == validAreaCollider) {
            inPlayArea = false;
        }
    }

    public bool InPlayArea() {
        return inPlayArea;
    }
}
