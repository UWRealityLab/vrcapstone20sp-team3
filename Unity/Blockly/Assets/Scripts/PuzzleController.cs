using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    public GameObject cursor;
    private CursorController cursorController;

    public GameObject puzzleBlockPrefab;

    private int selectedPuzzleId;  // index of currently selected puzzle
    private List<Module> puzzles;

    // Start is called before the first frame update
    void Start()
    {
        this.cursorController = cursor.GetComponent<CursorController>();
        this.puzzles = new List<Module>();

        // TODO: hardcoded level 1, maybe remove later
        Module level1 = new Module();
        level1.AddStatement("Right");
        level1.AddStatement("Right");
        level1.AddStatement("Up");
        level1.AddStatement("Forward");
        level1.AddStatement("Emit");
        level1.AddStatement("Right");
        level1.AddStatement("Emit");
        level1.AddStatement("Right");
        level1.AddStatement("Emit");
        level1.Complete();
        puzzles.Add(level1);
    }

    // Update is called once per frame
    // hardcoded keypress stuff
    void Update()
    {
        /* P - verify puzzle */
        if (Input.GetKeyDown(KeyCode.P))
        {
            Boolean result = VerifyPuzzle();
            Debug.Log("verify puzzle result: " + result);
        }
    }

    public void StartPuzzle(int puzzleId)
    {
        this.selectedPuzzleId = puzzleId;
        Vector3 puzzleCursorPosition = Vector3.zero;
        foreach (string statement in puzzles[puzzleId].Statements())
        {
            switch (statement)
            {
                case "Emit":
                    GameObject obj = Instantiate(this.puzzleBlockPrefab, puzzleCursorPosition, Quaternion.identity);
                    break;
                case "Right":
                    puzzleCursorPosition.x += CursorController.GRID_SIZE;
                    break;
                case "Left":
                    puzzleCursorPosition.x -= CursorController.GRID_SIZE;
                    break;
                case "Up":
                    puzzleCursorPosition.y += CursorController.GRID_SIZE;
                    break;
                case "Down":
                    puzzleCursorPosition.y -= CursorController.GRID_SIZE;
                    break;
                case "Forward":
                    puzzleCursorPosition.z += CursorController.GRID_SIZE;
                    break;
                case "Backward":
                    puzzleCursorPosition.z -= CursorController.GRID_SIZE;
                    break;
                default:
                    Debug.Log("unrecognized statement in puzzle level #" + puzzleId + ": " + statement);
                    break;
            }
        }
    }

    // verifies the user's blocks against the puzzle's target structure
    // returns true if correct, false if not
    public Boolean VerifyPuzzle()
    {
        for (float x = CursorController.MIN_POSITION; x <= CursorController.MAX_POSITION; x += CursorController.GRID_SIZE)
        {
            for (float y = CursorController.MIN_POSITION; y <= CursorController.MAX_POSITION; y += CursorController.GRID_SIZE)
            {
                for (float z = CursorController.MIN_POSITION; z <= CursorController.MAX_POSITION; z += CursorController.GRID_SIZE)
                {
                    Collider[] colliders = Physics.OverlapSphere(new Vector3(x, y, z), 0.2f);
                    Boolean userBlock = false;
                    Boolean puzzleBlock = false;
                    foreach (Collider collider in colliders)
                    {
                        if (collider.gameObject.tag == "Block")
                        {
                            userBlock = true;
                        } else if (collider.gameObject.tag == "Puzzle Block")
                        {
                            puzzleBlock = true;
                        }
                    }
                    if (userBlock != puzzleBlock)  // fail if user is missing block or has extra block
                    {
                        Debug.Log("verifying... user: " + userBlock + ", puzzle: " + puzzleBlock + " at " + new Vector3(x, y, z));
                        return false;
                    }
                }
            }
        }
        return true;
    }
}
