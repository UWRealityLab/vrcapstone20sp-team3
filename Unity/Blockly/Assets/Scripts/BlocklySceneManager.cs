using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BlocklySceneManager : MonoBehaviour {
  public void Start() {
    if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("BlocklyAesthetic")) {
      SceneManager.LoadScene("EnvironmentAesthetic", LoadSceneMode.Additive);
    }
  }
}
