using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Blockly {

public class PuzzleController : MonoBehaviour
{
    public GameObject cursor;
    private CursorController cursorController;

    public GameObject recordButton;
    private ModuleController moduleController;
    private bool userSubmittedWithModule;
    private bool moduleCorrect;  // only check this value if "userSubmittedWithModule" is true

    public GameObject puzzleBlockPrefab;
    public GameObject submitBlockPrefab;

    private int selectedPuzzleId;  // index of currently selected puzzle
    private Vector3 submitBlockPosition;
    private List<Module> puzzles;

    // Start is called before the first frame update
    void Start()
    {
        this.cursorController = cursor.GetComponent<CursorController>();
        this.moduleController = recordButton.GetComponent<ModuleController>();
        this.puzzles = new List<Module>();

        /* level 1 - 2 random emits */
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
        level1.AddStatement("Up");
        level1.AddStatement("Up");
        level1.AddStatement("Up");  // cursor end position ("submit" block)
        level1.Complete();
        puzzles.Add(level1);

        /* level 2 - a flat 2x2 square */
        Module level2 = new Module();
        level2.AddStatement("Emit");
        level2.AddStatement("Forward");
        level2.AddStatement("Emit");
        level2.AddStatement("Right");
        level2.AddStatement("Emit");
        level2.AddStatement("Backward");
        level2.AddStatement("Emit");
        level2.AddStatement("Up");
        level2.Complete();
        puzzles.Add(level2);

        /* level 3 - "stair" shape using the square from level 2 */
        Module level3 = new Module();
        for (int i = 0; i < 9; i++)
        {
            level3.AddStatement("Emit");
            level3.AddStatement("Forward");
            level3.AddStatement("Emit");
            level3.AddStatement("Right");
            level3.AddStatement("Emit");
            level3.AddStatement("Backward");
            level3.AddStatement("Emit");
            level3.AddStatement("Up");
            level3.Complete();
        }
        puzzles.Add(level3);
    }

    // Update is called once per frame
    void Update()
    {
        if (cursorController.CursorPosition() == this.submitBlockPosition)
        {
            if (moduleController.IsRecording()) // hopefully about to save module & submit
            {
                this.userSubmittedWithModule = true;
                this.moduleCorrect = VerifyPuzzle();
            }
        }
        else
        {
            // if they moved off the submit block without ending/saving the module
            if (this.userSubmittedWithModule && moduleController.IsRecording())
            {
                this.userSubmittedWithModule = false;
                this.moduleCorrect = false;
                Debug.Log("cursor moved off!");
            }
        }

        /* TODO: hardcoded for testing in play mode
         * P - verify puzzle
         */
        if (Input.GetKeyDown(KeyCode.P))
        {
            Boolean result = UserSubmittedCorrect();
            Debug.Log("user passed puzzle level?: " + result);
        }
    }

    public void StartPuzzle(int puzzleId)
    {
        this.selectedPuzzleId = puzzleId;
        this.userSubmittedWithModule = false;
        this.moduleCorrect = false;
        Vector3 puzzleCursorPosition = Vector3.zero;
        foreach (string statement in puzzles[puzzleId].Statements())
        {
            switch (statement)
            {
                case "Emit":
                    bool blockExisted = false;
                    Collider[] colliders = Physics.OverlapSphere(this.gameObject.transform.position, CursorController.GRID_SIZE / 3);
                    foreach (Collider collider in colliders)
                    {
                        if (collider.gameObject.tag == "Puzzle Block")
                        {
                            blockExisted = true;
                            Destroy(collider.gameObject);
                            break;
                        }
                    }

                    if (!blockExisted)
                    {
                        GameObject obj = Instantiate(this.puzzleBlockPrefab, puzzleCursorPosition, Quaternion.identity);
                    }
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
        Instantiate(this.submitBlockPrefab, puzzleCursorPosition, Quaternion.identity);  // add submit block
        this.submitBlockPosition = puzzleCursorPosition;
    }

    // verifies the user's blocks against the puzzle's target structure
    // returns true if correct, false if not
    private Boolean VerifyPuzzle()
    {
        Debug.Log("VerifyPuzzle()");
        Boolean needsModule = this.selectedPuzzleId == 1;  // for level 2, require module
        if (needsModule && !this.userSubmittedWithModule)
        {
            Debug.Log("VerifyPuzzle() - needsModule fail");
            return false;
        }
        Vector3 moduleLibraryPosition = this.moduleController.MostRecentModuleLibraryPosition();

        for (float x = CursorController.MIN_POSITION; x <= CursorController.MAX_POSITION; x += CursorController.GRID_SIZE)
        {
            for (float y = CursorController.MIN_POSITION; y <= CursorController.MAX_POSITION; y += CursorController.GRID_SIZE)
            {
                for (float z = CursorController.MIN_POSITION; z <= CursorController.MAX_POSITION; z += CursorController.GRID_SIZE)
                {
                    Collider[] colliders = Physics.OverlapSphere(new Vector3(x, y, z), CursorController.GRID_SIZE / 3);
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

                    if (userBlock != puzzleBlock)
                    {
                        Debug.Log("verify fail... user: " + userBlock + ", puzzle: " + puzzleBlock + " at " + new Vector3(x, y, z));
                        return false;
                    }
                }
            }
        }
        return true;
    }

    // returns true if the given puzzleId exists
    // puzzleId has to be >= 0
    public Boolean VerifyPuzzleId(int puzzleId)
    {
        return puzzleId < puzzles.Count;
    }

    // clears the main grid and puts cursor in original position
    public void ClearGrid()
    {
        List<GameObject> objectsToClear = new List<GameObject>(GameObject.FindGameObjectsWithTag("Block"));
        objectsToClear.AddRange(GameObject.FindGameObjectsWithTag("Puzzle Block"));
        objectsToClear.AddRange(GameObject.FindGameObjectsWithTag("Submit Block"));
        foreach (GameObject block in objectsToClear)
        {
            Destroy(block);
        }
        this.cursorController.gameObject.transform.position = new Vector3(CursorController.MIN_POSITION, CursorController.MIN_POSITION, CursorController.MIN_POSITION);
    }

    // return true if the user submitted a correct result (and they should pass the level)
    // false if not
    public Boolean UserSubmittedCorrect()
    {
        Boolean needsModule = this.selectedPuzzleId == 1;  // for level 2, require module
        if (this.moduleController.IsRecording())
        {
            return false;
        }
        if (needsModule)
        {
            return this.userSubmittedWithModule && this.moduleCorrect;
        }
        else
        {
            return this.cursorController.CursorPosition() == this.submitBlockPosition && this.VerifyPuzzle();
        }
    }
}

}
