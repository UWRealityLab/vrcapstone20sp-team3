using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blockly {

public class BlocklyGrid : MonoBehaviour {
  public static BlocklyGrid Instance = null;

  // number of cells along each axis of the grid
  public const int GRID_SIZE = 10;
  public const float LOCAL_CELL_SIZE = 1f / GRID_SIZE;
  public float GLOBAL_CELL_SIZE;

  [NotNull]
  public Transform gridSpace;

  void Awake() {
    Debug.Assert(Instance == null, "singleton class instantiated multiple times");
    Instance = this;
    gridSpace.localScale = Vector3.one / GRID_SIZE;
    // lossyScale == global scale
    GLOBAL_CELL_SIZE = gridSpace.lossyScale.x / GRID_SIZE;
  }

  public Collider[] BlocksAtIndex(Vector3Int index) {
    return BlocksAtPosition(gridSpace.TransformPoint(new Vector3(index.x, index.y, index.z)));
  }

  public Collider[] BlocksAtPosition(Vector3 pos) {
    return Physics.OverlapSphere(pos, GLOBAL_CELL_SIZE / 3);
  }
}

}
