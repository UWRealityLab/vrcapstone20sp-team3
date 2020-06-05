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

        public const string WELCOME_TITLE = "Welcome to Blockly Puzzle Mode";
        public const string WELCOME_INSTR = "Pinch to get rid of this popup!";

    void Awake() {
        Debug.Assert(Instance == null, "singleton class instantiated multiple times");
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        PopupMessage.Instance.ui.SetActive(false);
        SetButtonActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetButtonActive(bool active)
    {
        level1Button.SetActive(active);
        level2Button.SetActive(active);
        level3Button.SetActive(active);
    }

    public void BeginLevel1()
    {
        SetupLevel(-1);
    }

    public void BeginLevel2()
    {
        SetupLevel(0);
    }

    public void BeginLevel3()
    {
        SetupLevel(1);
    }

    private void SetupLevel(int level)
    {
        PopupMessage.Instance.SetLevel(level);
        PopupMessage.Instance.Open(WELCOME_TITLE, WELCOME_INSTR);
        SetButtonActive(false);
    }
 }

}
