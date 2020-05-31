using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blockly {

public class BlocklyDebug : MonoBehaviour {
    public static BlocklyDebug Instance;

    private bool inMenu;

    public void Awake() {
        Debug.Assert(Instance == null);
        Instance = this;
    }

    public static void Log(string msg) {
        Debug.Log($"blockly: {msg}");

        if (Instance != null) {
            Instance.CanvasLog(msg);
        }
    }

    public void CanvasLog(string msg) {
        // TODO implement
    }
}


}
