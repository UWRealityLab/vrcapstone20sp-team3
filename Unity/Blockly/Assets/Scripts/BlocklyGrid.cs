using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blockly {

public class BlocklyGrid : MonoBehaviour {
  public static BlocklyGrid Instance = null;

  [NotNull]
  public Transform gridSpace;

  void Awake() {
    Debug.Assert(Instance == null, "singleton class instantiated multiple times");
    Instance = this;
  }
}

}
