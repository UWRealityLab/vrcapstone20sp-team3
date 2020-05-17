using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocklyDebug : MonoBehaviour {
    public static BlocklyDebug Instance;

    public void Awake() {
        Debug.Assert(Instance == null);
        Instance = this;
    }
}
