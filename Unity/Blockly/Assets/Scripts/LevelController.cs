using Oculus.Platform.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    private GameObject level1Button;
    private GameObject level2Button;
    private GameObject level3Button;

    public GameObject levelPopup;
    private LevelPopupController popupController;

    //public GameObject popup;
    //private PopupMessage popupMessage;

    // Start is called before the first frame update
    void Start()
    {
        popupController = levelPopup.GetComponent<LevelPopupController>();
        //popupMessage = popup.GetComponent<PopupMessage>();
        level1Button = GameObject.Find("Level 1 Button");
        level2Button = GameObject.Find("Level 2 Button");
        level3Button = GameObject.Find("Level 3 Button");
        //popupMessage.Close();
        popupController.Close();
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
        //string dir = "Introduction to gestures\n";
        //dir += "1. Move a block to the right.\nPoint your pointer finger and drag in desired direction.\n";
        //dir += "2. Emit block at cursor.\nStart with a fist and release,\nmaking a 5 with your fingers, hand facing down.";
        SetupLevel(-1);
    }

    public void BeginLevel2()
    {
        //string dir = "Module\nTap on recording button to start recording your module.\n";
        //dir += "Emit a block and move cursor to the right.\n Finish by pressing recording button again.";
        SetupLevel(0);
    }

    public void BeginLevel3()
    {
        //string dir = "Loop\nChoose module and loop over it ten times in a clockwise circular motion.\n";
        //dir += "Apply module to main grid.";
        SetupLevel(1);
    }

    private void SetupLevel(int level)
    {
        popupController.SetLevel(level);
        popupController.Open("Welcome to Blockly Puzzle Mode", "Press <TODO> to continue", "");
        SetButtonActive(false);
        SceneManager.LoadScene("Blockly", LoadSceneMode.Additive);
        //SceneManager.LoadScene("GestureRecognition", LoadSceneMode.Additive);
    }
}
