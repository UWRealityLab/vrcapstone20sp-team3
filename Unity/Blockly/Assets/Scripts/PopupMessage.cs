﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupMessage : MonoBehaviour
{
    public GameObject ui;
    private PuzzleController puzzleController;
    private bool verified;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        clickButton();
        if (puzzleController != null && !verified)
        {
            MoveToNextLevel();
        }
    }

    public void Open(string message)
    {
        ui.SetActive(!ui.activeSelf);
        if (ui.activeSelf)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Text textObject = ui.gameObject.GetComponentInChildren<Text>();
                textObject.text = message;
            }
        }
    }

    public void Close()
    {
        ui.SetActive(!ui.activeSelf);

        puzzleController = GameObject.Find("Puzzle Controller").GetComponent<PuzzleController>();
        puzzleController.StartPuzzle(0);
    }

    private void clickButton()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Button okButton = GameObject.Find("OK").GetComponent<Button>();
            okButton.onClick.Invoke();
        }
    }

    public void MoveToNextLevel()
    {
        if (puzzleController.VerifyPuzzle())
        {
            Open("Do you want to move on to the next level?");
            verified = true;
        }
    }
}
