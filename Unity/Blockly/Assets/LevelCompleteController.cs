using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleteController : MonoBehaviour
{
    public GameObject levelCompleteWindow;
    
    void Start()
    {
        ClearCompleteLevelPopUp();
    }

    public void CompleteLevelPopUp()
    {
        // TODO: Change from toggling
        if (levelCompleteWindow.activeSelf)
        {
            ClearCompleteLevelPopUp();
        } else {
            levelCompleteWindow.SetActive(true);
        }
    }

    public void ClearCompleteLevelPopUp()
    {
        levelCompleteWindow.SetActive(false);
    }
}
