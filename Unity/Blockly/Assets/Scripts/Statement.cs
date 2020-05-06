using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statement : MonoBehaviour
{
    public string name;  // gesture name or module name
    public bool isGesture;  // true if a simple action, false if a module

    public Statement(string name, bool isGesture)
    {
        this.name = name;
        this.isGesture = isGesture;
    }
}
