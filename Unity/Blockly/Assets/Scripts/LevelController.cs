using Oculus.Platform.Models;
using System.Collections;
using System.Collections.Generic;
// using System.Diagnostics;
using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;

namespace Blockly {

public class LevelController : MonoBehaviour
{
  public static LevelController Instance = null;

  [SerializeField] [NotNull]
    private GameObject level1Button;
  [SerializeField] [NotNull]
    private GameObject level2Button;
  [SerializeField] [NotNull]
    private GameObject level3Button;

    private int currentLevel;
    private bool verified;
    private bool levelSet;

    public const string WELCOME_TITLE = "Welcome to Blockly Puzzle Mode";
    public const string WELCOME_INST = "Pinch to get rid of this popup!";

    public const string INSTRUCTION = "Fill in the green outline with blocks. Place cursor at yellow box to submit.";

    public const string NEXT_LEVEL_TITLE = "LEVEL PASSED!\n";
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

    void Awake() {
        Debug.Assert(Instance == null, "singleton class instantiated multiple times");
        Instance = this;

        titles = new List<string>();
        titles.Add(LEVEL1_TITLE);
        titles.Add(LEVEL2_TITLE);
        titles.Add(LEVEL3_TITLE);

        instructions = new List<string>();
        instructions.Add(LEVEL1_INST + INSTRUCTION);
        instructions.Add(LEVEL2_INST + LEVEL2_MODULE_INST);
        instructions.Add(LEVEL3_INST + INSTRUCTION);
    }

    // Start is called before the first frame update
    void Start()
    {
        currentLevel = -1;
        verified = false;
        PopupMessage.Instance.ui.SetActive(false);
        SetButtonActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (PuzzleController.Instance != null && !verified)
        {
            MoveToNextLevel();
        }
    }

    public void SetButtonActive(bool active)
    {
        level1Button.SetActive(active);
        level2Button.SetActive(active);
        level3Button.SetActive(active);
    }

    public void BeginLevel1()
    {
        StartLevel(0);
    }

    public void BeginLevel2()
    {
        StartLevel(1);
    }

    public void BeginLevel3()
    {
        StartLevel(2);
    }

    public void MoveToNextLevel()
    {
        if (PuzzleController.Instance.UserSubmittedCorrect())
        {
            verified = true;
            currentLevel++;
            if (PuzzleController.Instance.VerifyPuzzleId(currentLevel))
            {
                this.levelSet = false;
                string t = NEXT_LEVEL_TITLE + "UP NEXT: " + titles[currentLevel];
                PopupMessage.Instance.Open(t, instructions[currentLevel]);
            }
            else
            {
                PopupMessage.Instance.Open(CONGRATS_TITLE, CONGRATS_INST);
                PuzzleController.Instance.ClearGrid();
            }
        }
    }

    private void StartLevel(int level)
    {
        this.currentLevel = level;
        this.levelSet = false;
        PopupMessage.Instance.Open(titles[currentLevel], instructions[currentLevel]);
        SetButtonActive(false);
    }

    public void SetupLevel()
    {
        if (!levelSet && PuzzleController.Instance.VerifyPuzzleId(currentLevel))
        {
            PuzzleController.Instance.ClearGrid();
            PuzzleController.Instance.StartPuzzle(currentLevel);
            levelSet = true;
            verified = false;
        }
    }
 }

}
