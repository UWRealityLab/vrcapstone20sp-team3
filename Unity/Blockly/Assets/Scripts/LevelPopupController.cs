using Blockly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class LevelPopupController : MonoBehaviour
{
    public GameObject ui;
    private PuzzleController puzzleController;
    private bool verified;
    private int currentLevel;

    private List<string> titles;
    private List<string> instructions;

    public const string INSTRUCTION = "Fill in the green outline with blocks. Place cursor at yellow box to submit.";

    public const string NEXT_LEVEL_TITLE = "LEVEL PASSED!";
    public const string NEXT_LEVEL_INST = "Make a pinch gesture to move to the next level!";

    public const string CONGRATULATIONS = "CONGRATULATIONS!";
    public const string END_INST = " You will now enter free play mode, where you can explore your creativity in 3D :O";

    public const string LEVEL1_TITLE = "LEVEL 1: INTRODUCTION";
    public const string LEVEL1_INST = "To move block, point your pointer finger and drag in desired direction." +
        "To emit block at cursor, start with a fist and release,\nmaking a 5 with your fingers, hand facing down.";

    public const string LEVEL2_TITLE = "LEVEL 2: MODULE";
    public const string LEVEL2_INST = "To create a module, fold your hand with only thumb on bottom for both hands.";
    public const string LEVEL2_MODULE_INST = "All of the modules you define in this level will be available to you in the" +
        "module library for use in future levels. Good luck!";

    public const string LEVEL3_TITLE = "LEVEL 3: LoOpY StAirS";
    public const string LEVEL3_INST = "To loop, select module by making a pinch gesture and " +
        "loop over it desired number of times in a clockwise circular motion. " +
        "Apply module by dragging pinched module from module library to the play area.";

     void Start()
    {
        currentLevel = -1;
        verified = true;
        titles = new List<string>();
        instructions = new List<string>();

        titles.Add(LEVEL1_TITLE);
        titles.Add(LEVEL2_TITLE);
        titles.Add(LEVEL3_TITLE);

        instructions.Add(LEVEL1_INST);
        instructions.Add(LEVEL2_INST);
        instructions.Add(LEVEL3_INST);
    }

    private void Update()
    {
        closePopup();
        if (puzzleController != null && !verified)
        {
            MoveToNextLevel();
        }
    }

    public void SetLevel(int level)
    {
        this.currentLevel = level;
    }

    public void Open(string title, string instruction, string module)
    {
        ui.SetActive(!ui.activeSelf);
        if (ui.activeSelf)
        {
            setText(title, 0);
            setText(instruction, 1);
            setText(module, 2);

        }
    }

    public void Close()
    {
        ui.SetActive(!ui.activeSelf);

        if (puzzleController == null)
        {
            puzzleController = GameObject.Find("Puzzle Controller").GetComponent<PuzzleController>();
        }

        if (verified)
        {
            if (currentLevel >= 0)
            {
                puzzleController.ClearGrid();
            }
            if (puzzleController.VerifyPuzzleId(currentLevel + 1))
            {
                Debug.Log("incrementing current level");
                currentLevel++;
                if (currentLevel == 1)
                {
                    Open(titles[currentLevel], instructions[currentLevel], INSTRUCTION + LEVEL2_MODULE_INST);
                } else
                {
                    Open(titles[currentLevel], instructions[currentLevel], INSTRUCTION);
                }
                puzzleController.StartPuzzle(currentLevel);
                verified = false;
            }
        }
    }

    public void MoveToNextLevel()
    {
        if (puzzleController.UserSubmittedCorrect())
        {
            Debug.Log("successfully completed level " + currentLevel);
            verified = true;
            if (puzzleController.VerifyPuzzleId(currentLevel + 1))
            {
                Open(NEXT_LEVEL_TITLE, NEXT_LEVEL_INST, "");

            } else
            {
                Open(CONGRATULATIONS, END_INST, "");
            }
        }
    }

    private void closePopup()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Close();
        }
    }

    private void setText(string message, int childNum)
    {
        if (message != null)
        {
            Text textObject = ui.transform.GetChild(childNum).gameObject.GetComponentInChildren<Text>();
            textObject.text = message;
        }
    }
}
