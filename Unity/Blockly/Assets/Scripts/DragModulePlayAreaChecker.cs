using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragModulePlayAreaChecker : MonoBehaviour {
    private BoxCollider collider;
    private BoxCollider validAreaCollider;

    private bool inPlayArea = false;

    public void Awake() {
        collider = GetComponent<BoxCollider>();
        Debug.Assert(collider != null, "missing box collider component on object");
        GameObject validBlockAreaObj = GameObject.FindGameObjectWithTag("Valid Block Area");
        Debug.Assert(validBlockAreaObj != null, "no object tagged with \"Valid Block Area\" in scene");
        validAreaCollider = validBlockAreaObj.GetComponent<BoxCollider>();
        Debug.Assert(validBlockAreaObj != null, "missing box collider on object tagged with \"Valid Block Area\" in scene");
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
