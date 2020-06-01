using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private GameObject puzzleButton;
    private GameObject freePlayButton;

    public GameObject level;
    private LevelController levelController;

    private void Start()
    {
        levelController = level.GetComponent<LevelController>();
        puzzleButton = GameObject.Find("Puzzle Button");
        freePlayButton = GameObject.Find("Free Play Button");
        levelController.SetButtonActive(false);
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

    private void LoadScene()
    {
        SceneManager.LoadScene("Blockly", LoadSceneMode.Additive);
        //SceneManager.LoadScene("GestureRecognition", LoadSceneMode.Additive);
    }
}