using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blockly {

public class ModuleLibrary : MonoBehaviour {
  public static ModuleLibrary Instance;

  private int numModules;

  public void Awake() {
    Debug.Assert(Instance == null);
    Instance = this;
    numModules = 0;
  }

  public void AddModule(int moduleName, GameObject moduleMeshObj) {
    Debug.Log($"looking for child {numModules}");
    GameObject libModObj = gameObject.transform.Find($"Modules/{numModules}").gameObject;
    libModObj.GetComponent<BlocklyLibraryModule>().moduleName = moduleName;
    GameObject libModMeshObj = libModObj.transform.Find($"Mesh").gameObject;
    moduleMeshObj.transform.parent = libModMeshObj.transform;
    moduleMeshObj.transform.localPosition = Vector3.zero;

    numModules++;
  }
}

}
