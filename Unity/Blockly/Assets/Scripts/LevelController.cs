using Oculus.Platform.Models;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Blockly {

public class LevelController : MonoBehaviour
{
    private GameObject level1Button;
    private GameObject level2Button;
    private GameObject level3Button;

    public GameObject popup;
    private PopupMessage popupMessage;

    // Start is called before the first frame update
    void Start()
    {
        popupMessage = popup.GetComponent<PopupMessage>();
        level1Button = GameObject.Find("Level 1 Button");
        level2Button = GameObject.Find("Level 2 Button");
        level3Button = GameObject.Find("Level 3 Button");
        popupMessage.Close();
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
        SetupLevel(-1);
    }

    private void SetupLevel(int level)
    {
        popupMessage.SetLevel(level);
        popupMessage.Open("Welcome to Blockly Puzzle Mode");
        SetButtonActive(false);
        SceneManager.LoadScene("Environment", LoadSceneMode.Additive);
    }
 }

}
