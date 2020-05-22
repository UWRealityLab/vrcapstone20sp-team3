using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragModulePlayAreaChecker : MonoBehaviour {
    private BoxCollider collider;
    private BoxCollider validAreaCollider;

    private bool inPlayArea = false;

    public void Awake() {
        collider = GetComponent<BoxCollider>();
        GameObject validBlockAreaObj = GameObject.FindGameObjectWithTag("Valid Block Area");
        Debug.Log($"Valid Block Area: {validBlockAreaObj}");
        validAreaCollider = validBlockAreaObj.GetComponent<BoxCollider>();
        Debug.Log($"Valid Area Collider: {validAreaCollider}");
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
