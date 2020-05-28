using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupMessage : MonoBehaviour
{
    public GameObject ui;
    private PuzzleController puzzleController;
    private bool verified;
    private int currentLevel;

    // Start is called before the first frame update
    void Start()
    {
        currentLevel = -1;
        verified = true;
    }

    // Update is called once per frame
    void Update()
    {
        clickButton();
        if (puzzleController != null && !verified && puzzleController.UserSubmitted())
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
        if (puzzleController.VerifyPuzzle())
        {
            Open("Do you want to move on to the next level?");
            verified = true;
        }
    }
}
