using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

namespace Blockly {

public class PopupMessage : MonoBehaviour
{
    public GameObject ui;

    private PuzzleController puzzleController;
    private bool verified;
    private int currentLevel;

    public const string INSTRUCTION = "Fill in the green outline with blocks. Place cursor at yellow box to submit.";

    public const string NEXT_LEVEL_TITLE = "LEVEL PASSED!";
    public const string NEXT_LEVEL_INST = "Make a pinch gesture to move to the next level!";

    public const string CONGRATS_TITLE = "CONGRATULATIONS!";
    public const string CONGRATS_INST = " You will now enter free play mode, where you can explore your creativity in 3D!";

    public const string LEVEL1_TITLE = "LEVEL 1: Introductory Statements";
    public const string LEVEL1_INST = "To move block, point your pointer finger and drag in desired direction.\n" +
    "To emit block at cursor, start with a fist and release, making a 5 with your fingers, hand facing down.\n\n";

    public const string LEVEL2_TITLE = "LEVEL 2: MODULE";
    public const string LEVEL2_INST = "To create a module, fold your hand with only thumb on bottom for both hands.\n\n";
    public const string LEVEL2_MODULE_INST = "All of the modules you define in this level will be available to you in the" +
        "module library for use in future levels. Good luck!";

    public const string LEVEL3_TITLE = "LEVEL 3: LoOpY StAirS";
    public const string LEVEL3_INST = "To loop, select module by making a pinch gesture and " +
    "loop over it desired number of times in a clockwise circular motion.\n" +
    "Apply module by dragging pinched module from module library to the play area.\n\n";


        private List<string> titles;
    private List<string> instructions;


    // Start is called before the first frame update
    void Start()
    {
        currentLevel = -1;
        verified = true;

        titles = new List<string>();
        titles.Add(LEVEL1_TITLE);
        titles.Add(LEVEL2_TITLE);
        titles.Add(LEVEL3_TITLE);

        instructions = new List<string>();
        instructions.Add(LEVEL1_INST + INSTRUCTION);
        instructions.Add(LEVEL2_INST + LEVEL2_MODULE_INST);
        instructions.Add(LEVEL3_INST + INSTRUCTION);
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

    public void SetLevel(int level)
    {
        this.currentLevel = level;
    }

    public void Open(string title, string instruction)
    {
        ui.SetActive(!ui.activeSelf);
        if (ui.activeSelf)
        {
            setText(title, 0);
            setText(instruction, 1);
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
                currentLevel++;
                Open(titles[currentLevel], instructions[currentLevel]);
                puzzleController.StartPuzzle(currentLevel);
                verified = false;
            } // an else which means they are completed with all possible levels --> give them a congratulations :D
        }
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
        if (puzzleController.UserSubmittedCorrect())
        {
            verified = true;
            if (puzzleController.VerifyPuzzleId(currentLevel + 1))
            {
                Open(NEXT_LEVEL_TITLE, NEXT_LEVEL_INST);
            }
            else
            {
                Open(CONGRATS_TITLE, CONGRATS_INST);
            }
        }
    }

    private void setText(string message, int childNum)
    {
        if (message != null)
        {
            Text[] textObject = ui.gameObject.GetComponentsInChildren<Text>();
            textObject[childNum].text = message;
        }
    }
}

}
