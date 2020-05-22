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
    private GameObject level1Button;
    private GameObject level2Button;
    private GameObject level3Button;

    private void Start()
    {
        puzzleButton = GameObject.Find("Puzzle Button");
        freePlayButton = GameObject.Find("Free Play Button");
        level1Button = GameObject.Find("Level 1 Button");
        level2Button = GameObject.Find("Level 2 Button");
        level3Button = GameObject.Find("Level 3 Button");
        SetButtonActive(false);
    }

    public void BeginPuzzleMode()
    {
        //puzzleButton.gameObject.GetComponent<Renderer>().enabled = f;
        puzzleButton.SetActive(false);
        SetButtonActive(true);
        //freePlayButton.gameObject.GetComponent<Renderer>().enabled = true;
        freePlayButton.SetActive(false);
    }

    public void BeginFreePlayMode()
    {
        SceneManager.LoadScene("Blockly", LoadSceneMode.Additive);
        //SceneManager.LoadScene("GestureRecognition", LoadSceneMode.Additive);
        puzzleButton.SetActive(false);
        freePlayButton.SetActive(false);
    }

    public void BeginLevel1()
    {
        SetButtonActive(false);
        LoadScene();
    }

    public void BeginLevel2()
    {
        SetButtonActive(false);
        LoadScene();
    }

    public void BeginLevel3()
    {
        SetButtonActive(false);
        LoadScene();
    }

    private void LoadScene()
    {
        SceneManager.LoadScene("Blockly", LoadSceneMode.Additive);
        //SceneManager.LoadScene("GestureRecognition", LoadSceneMode.Additive);
    }

    public void SetButtonActive(bool active)
    {
        level1Button.SetActive(active);
        level2Button.SetActive(active);
        level3Button.SetActive(active);
    }
}