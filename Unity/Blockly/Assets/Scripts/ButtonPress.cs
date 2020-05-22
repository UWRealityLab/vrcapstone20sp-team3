using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonPress : MonoBehaviour
{
    public UnityEvent onTriggered;

     void OnTriggerEnter(Collider Col)
    {
        onTriggered.Invoke();
    }
}
