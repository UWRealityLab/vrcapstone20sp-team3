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
  [SerializeField] [NotNull]
    private GameObject puzzleButton;
  [SerializeField] [NotNull]
    private GameObject freePlayButton;

    public GameObject level;

  void Awake() {
    Debug.Assert(Instance == null, "singleton class instantiated multiple times");
    Instance = this;
  }

    private void Start()
    {
        LevelController.Instance.SetButtonActive(false);
    }

    public void BeginPuzzleMode()
    {
        puzzleButton.SetActive(false);
        freePlayButton.SetActive(false);
        LevelController.Instance.SetButtonActive(true);
    }

    public void BeginFreePlayMode()
    {
        puzzleButton.SetActive(false);
        freePlayButton.SetActive(false);
    }
}

}
