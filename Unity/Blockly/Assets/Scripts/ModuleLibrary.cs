using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blockly {

public class ModuleLibrary : MonoBehaviour {
  public static ModuleLibrary Instance;

  private static int MODULES_PER_SHELF = 5;
  private static int MAX_MODULES = 15;

  [SerializeField] [NotNull]
  private GameObject libraryModulePrefab;
  [SerializeField] [NotNull]
  // ordered from top to bottom shelf
  private Transform[] modulesContainerTransforms;

  [SerializeField]
  private BoxCollider moduleAllowedBoundsCol;
  public Bounds ModuleAllowedBounds {
    get {
      return moduleAllowedBoundsCol.bounds;
    }
  }

  private int numModules;

  public void Awake() {
    Debug.Assert(Instance == null);
    Debug.Assert(moduleAllowedBoundsCol != null);
    Instance = this;
    numModules = 0;
  }

  public void AddModule(int moduleName, GameObject modBlockMeshObj) {
    Debug.Assert(numModules < MAX_MODULES);
    Transform modulesContainerTransform = modulesContainerTransforms[numModules / MODULES_PER_SHELF];
    GameObject libModObj = Instantiate(libraryModulePrefab, modulesContainerTransform);
    libModObj.transform.localPosition = new Vector3(0f, 0f, (numModules % MODULES_PER_SHELF) * 0.43f);

    libModObj.GetComponent<BlocklyLibraryModule>().moduleName = moduleName;
    GameObject libModMeshObj = libModObj.transform.Find($"Mesh").gameObject;
    CenterAndScaleModule(modBlockMeshObj);
    modBlockMeshObj.transform.parent = libModMeshObj.transform;
    modBlockMeshObj.transform.localPosition = Vector3.zero;

    numModules++;
  }

  private void CenterAndScaleModule(GameObject modBlockMeshObj) {
    // find bounding box of module
    var colliders = modBlockMeshObj.GetComponentsInChildren<Collider>();
    Bounds meshBounds = colliders[0].bounds;
    foreach(var c in colliders) meshBounds.Encapsulate(c.bounds);

    Debug.Log($"meshBounds.center: {meshBounds.center}");
    Debug.Log($"allowedBounds.center: {ModuleAllowedBounds.center}");
    // recenter blocks within modBlockMeshObj
    Vector3 delta = -meshBounds.center;
    Transform[] children = modBlockMeshObj.transform.GetComponentsInChildren<Transform>();
    foreach (Transform child in children) {
      if (child.parent == modBlockMeshObj.transform) {
        child.transform.position += delta;
      }
    }

    // scale according to calculated bounding box so it fits in the library module's
    // box collider
    var szA = ModuleAllowedBounds.size;
    var szB = meshBounds.size;
    float[] scales = {szA.x / szB.x, szA.y / szB.y, szA.z / szB.z};
    modBlockMeshObj.transform.localScale *= scales.Min();
  }

}

}
