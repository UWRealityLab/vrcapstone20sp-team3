using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Blockly {

public class BlocklySceneManager : MonoBehaviour {
  public void Start() {
    if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("BlocklyAesthetic")) {
      SceneManager.LoadScene("EnvironmentAesthetic", LoadSceneMode.Additive);
    }
    StartCoroutine(Progress());
    // PuzzleController.Instance.StartPuzzle(2);
    // PuzzleController.Instance.StartPuzzle(2);
  }

  private IEnumerator Progress() {
    MenuController.Instance.BeginPuzzleMode();
    yield return new WaitForSeconds(1f);
    LevelController.Instance.BeginLevel3();
    yield return new WaitForSeconds(1f);
    PopupMessage.Instance.ClickButton();
    yield return null;
  }

}

}
