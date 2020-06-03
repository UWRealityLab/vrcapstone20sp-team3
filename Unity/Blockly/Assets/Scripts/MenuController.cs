using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Blockly {

public class MenuController : MonoBehaviour
{
  public static MenuController Instance = null;
    private GameObject puzzleButton;
    private GameObject freePlayButton;

    public GameObject level;
    private LevelController levelController;

  void Awake() {
    Debug.Assert(Instance == null, "singleton class instantiated multiple times");
    Instance = this;

        levelController = level.GetComponent<LevelController>();
        puzzleButton = GameObject.Find("Puzzle Button");
        freePlayButton = GameObject.Find("Free Play Button");
  }

    private void Start()
    {
        levelController.SetButtonActive(false);
        // SceneManager.LoadScene("Environment", LoadSceneMode.Additive);
    }

    public void BeginPuzzleMode()
    {
        puzzleButton.SetActive(false);
        levelController.SetButtonActive(true);
        freePlayButton.SetActive(false);
    }

    public void BeginFreePlayMode()
    {
        puzzleButton.SetActive(false);
        freePlayButton.SetActive(false);
    }
}

}
