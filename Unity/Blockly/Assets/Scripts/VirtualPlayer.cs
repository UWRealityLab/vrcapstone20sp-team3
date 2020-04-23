using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blockly {

public class VirtualPlayer : MonoBehaviour {
  public GameObject questPlayer;
  public GameObject editorPlayer;

  public GameObject chosenPlayer;

  public void Awake() {
    #if UNITY_EDITOR
    Debug.Log("chose editor player");
    editorPlayer.SetActive(true);
    questPlayer.SetActive(false);
    chosenPlayer = editorPlayer;
    #else
    Debug.Log("chose Quest player");
    editorPlayer.SetActive(false);
    questPlayer.SetActive(true);
    chosenPlayer = questPlayer;
    #endif
  }
}

}
