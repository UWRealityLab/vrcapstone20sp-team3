using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statement : MonoBehaviour
{
    public string name;  // gesture name or module name
    public bool isGesture;  // true if a simple action, false if a module
    public GameObject module;  // only used for looping statement - module to loop over
    public int times;  // only used for looping statement - number of times to loop

    public Statement(string name, bool isGesture) : this(name, isGesture, null, 0) { }

    public Statement(string name, bool isGesture, GameObject module, int times)
    {
        this.name = name;
        this.isGesture = isGesture;
        this.module = module;
        this.times = times;
    }

    public override string ToString()
    {
        return this.name;
    }

    public void SetModule(GameObject loopedModule)
    {
        this.module = loopedModule;
    }
}
