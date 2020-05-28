﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupMessage : MonoBehaviour
{
    public GameObject ui;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        clickButton();
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

        PuzzleController puzzleController = GameObject.Find("Puzzle Controller").GetComponent<PuzzleController>();
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
}