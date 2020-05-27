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

    public const float GRID_SIZE = 0.5f;

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
        StartPuzzle(0);
    }

    // Update is called once per frame
    void Update()
    {
        
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
                    puzzleCursorPosition.x += GRID_SIZE;
                    break;
                case "Left":
                    puzzleCursorPosition.x -= GRID_SIZE;
                    break;
                case "Up":
                    puzzleCursorPosition.y += GRID_SIZE;
                    break;
                case "Down":
                    puzzleCursorPosition.y -= GRID_SIZE;
                    break;
                case "Forward":
                    puzzleCursorPosition.z += GRID_SIZE;
                    break;
                case "Backward":
                    puzzleCursorPosition.z -= GRID_SIZE;
                    break;
                default:
                    Debug.Log("unrecognized statement in puzzle level #" + puzzleId + ": " + statement);
                    break;
            }
        }
    }
}
