using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Blockly {

public class PuzzleController : MonoBehaviour
{
  public static PuzzleController Instance = null;

    public GameObject cursor;
    // private CursorController CursorController.Instance;

    public GameObject recordButton;
    // private ModuleController ModuleController.Instance;
    private bool userSubmittedWithModule;
    private bool moduleCorrect;  // only check this value if "userSubmittedWithModule" is true

    public GameObject puzzleBlockPrefab;
    public GameObject submitBlockPrefab;

    private int selectedPuzzleId;  // index of currently selected puzzle
    private Vector3 submitBlockPosition;
    private List<Module> puzzles;

  public void Awake() {
    Debug.Assert(Instance == null, "singleton class instantiated multiple times");
    Instance = this;

        this.puzzles = new List<Module>();

        /* level 1 - 2 random emits */
        Module level1 = new Module();
        level1.AddStatement("Right");
        level1.AddStatement("Right");
        level1.AddStatement("Up");
        level1.AddStatement("Forward");
        level1.AddStatement("Emit");
        level1.AddStatement("Right");
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
        level2.AddStatement("Forward");
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
            level3.AddStatement("Forward");
            level3.AddStatement("Up");
            level3.Complete();
        }
        puzzles.Add(level3);
  }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (CursorController.Instance.CursorPosition() == this.submitBlockPosition)
        {
            if (ModuleController.Instance.IsRecording()) // hopefully about to save module & submit
            {
                this.userSubmittedWithModule = true;
                this.moduleCorrect = VerifyPuzzle();
            }
        }
        else
        {
            // if they moved off the submit block without ending/saving the module
            if (this.userSubmittedWithModule && ModuleController.Instance.IsRecording())
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
        Vector3Int puzzleCursorIdx = Vector3Int.zero;
        foreach (string statement in puzzles[puzzleId].Statements())
        {
            switch (statement)
            {
                case "Emit":
                    bool blockExisted = false;
                    Collider[] colliders = BlocklyGrid.Instance.BlocksAtIndex(puzzleCursorIdx);
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
                        GameObject obj = Instantiate(puzzleBlockPrefab, BlocklyGrid.Instance.PositionFromIdx(puzzleCursorIdx), Quaternion.identity, BlocklyGrid.Instance.gridSpace) as GameObject;
                    }
                    break;
                case "Right":
                    puzzleCursorIdx.x++;
                    break;
                case "Left":
                    puzzleCursorIdx.x--;
                    break;
                case "Up":
                    puzzleCursorIdx.y++;
                    break;
                case "Down":
                    puzzleCursorIdx.y--;
                    break;
                case "Forward":
                    puzzleCursorIdx.z++;
                    break;
                case "Backward":
                    puzzleCursorIdx.z--;
                    break;
                default:
                    Debug.Log("unrecognized statement in puzzle level #" + puzzleId + ": " + statement);
                    break;
            }
        }

        // add submit block
        Instantiate(submitBlockPrefab, BlocklyGrid.Instance.PositionFromIdx(puzzleCursorIdx), Quaternion.identity, BlocklyGrid.Instance.gridSpace);
        this.submitBlockPosition = BlocklyGrid.Instance.PositionFromIdx(puzzleCursorIdx);
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
        // Vector3 moduleLibraryPosition = ModuleController.Instance.MostRecentModuleLibraryPosition();

        for (int x = 0; x <= BlocklyGrid.GRID_SIZE; x++)
        {
            for (int y = 0; y <= BlocklyGrid.GRID_SIZE; y++)
            {
                for (int z = 0; z <= BlocklyGrid.GRID_SIZE; z++)
                {
                    // Collider[] colliders = Physics.OverlapSphere(CursorController.Instance.PositionFromCursorIdx(new Vector3Int(x, y, z)), BlocklyGrid.GLOBAL_CELL_SIZE / 3);
                    Collider[] colliders = BlocklyGrid.Instance.BlocksAtIndex(new Vector3Int(x, y, z));
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
        CursorController.Instance.gameObject.transform.localPosition = Vector3.zero;
    }

    // return true if the user submitted a correct result (and they should pass the level)
    // false if not
    public Boolean UserSubmittedCorrect()
    {
        Boolean needsModule = this.selectedPuzzleId == 1;  // for level 2, require module
        if (ModuleController.Instance.IsRecording())
        {
            return false;
        }
        if (needsModule)
        {
            return this.userSubmittedWithModule && this.moduleCorrect;
        }
        else
        {
            return CursorController.Instance.CursorPosition() == this.submitBlockPosition && this.VerifyPuzzle();
        }
    }
}

}
