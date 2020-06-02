using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Blockly {

public class MenuController : MonoBehaviour
{
    private GameObject puzzleButton;
    private GameObject freePlayButton;
    //private GameObject level1Button;
    //private GameObject level2Button;
    //private GameObject level3Button;

    public GameObject level;
    private LevelController levelController;

    private void Start()
    {
        levelController = level.GetComponent<LevelController>();
        puzzleButton = GameObject.Find("Puzzle Button");
        freePlayButton = GameObject.Find("Free Play Button");
        levelController.SetButtonActive(false);

        puzzleButton.SetActive(false);
        freePlayButton.SetActive(false);
        SceneManager.LoadScene("Blockly", LoadSceneMode.Additive);
    }

    public void BeginPuzzleMode()
    {
        puzzleButton.SetActive(false);
        levelController.SetButtonActive(true);
        freePlayButton.SetActive(false);
    }

    public void BeginFreePlayMode()
    {
        SceneManager.LoadScene("Blockly", LoadSceneMode.Additive);
        //SceneManager.LoadScene("GestureRecognition", LoadSceneMode.Additive);
        puzzleButton.SetActive(false);
        freePlayButton.SetActive(false);
    }

    //public void BeginLevel1()
    //{
    //    popupMessage.Open("Step 1: Emit a block.");

    //    SetButtonActive(false);
    //    LoadScene();
    //}

    //public void BeginLevel2()
    //{
    //    SetButtonActive(false);
    //    LoadScene();
    //}

    //public void BeginLevel3()
    //{
    //    SetButtonActive(false);
    //    LoadScene();
    //}

    private void LoadScene()
    {
        SceneManager.LoadScene("Blockly", LoadSceneMode.Additive);
        //SceneManager.LoadScene("GestureRecognition", LoadSceneMode.Additive);
    }

    //public void SetButtonActive(bool active)
    //{
    //    level1Button.SetActive(active);
    //    level2Button.SetActive(active);
    //    level3Button.SetActive(active);
    //}
}

}
