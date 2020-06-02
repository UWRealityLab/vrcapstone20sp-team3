using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Blockly {

public class PopupMessage : MonoBehaviour
{
    public GameObject ui;
    private PuzzleController puzzleController;
    private bool verified;
    private int currentLevel;

    public const string LEVEL1_MESSAGE = "Introduction to gestures\n" +
        "1. Move a block to the right.\nPoint your pointer finger and drag in desired direction.\n" +
        "2. Emit block at cursor.\nStart with a fist and release,\nmaking a 5 with your fingers, hand facing down.";

    public const string LEVEL2_MESSAGE = "Module\nTap on recording button to start recording your module.\n" +
        "Emit a block and move cursor to the right.\n Finish by pressing recording button again.";

    public const string LEVEL3_MESSAGE = "Loop\nChoose module and loop over it ten times in a clockwise circular motion.\n" +
        "Apply module to main grid.";

    private List<string> levelMessages;

    // Start is called before the first frame update
    void Start()
    {
        currentLevel = -1;
        verified = true;
        levelMessages = new List<string>();
        levelMessages.Add(LEVEL1_MESSAGE);
        levelMessages.Add(LEVEL2_MESSAGE);
        levelMessages.Add(LEVEL3_MESSAGE);
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

    public void Open(string message)
    {
        ui.SetActive(!ui.activeSelf);
        if (ui.activeSelf)
        {
            if (message != null)
            {
                Text textObject = ui.gameObject.GetComponentInChildren<Text>();
                textObject.text = message;
            }
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
                Open(levelMessages[currentLevel]);
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
                Open("LEVEL PASSED");
            }
            else
            {
                Open("CONGRATULATIONS");
            }
        }
    }
}

}
