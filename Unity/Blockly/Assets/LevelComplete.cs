using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelComplete : MonoBehaviour
{
    public GameObject levelCompleteWindow;
    
    void Start()
    {
        levelCompleteWindow.SetActive(false);
    }

    // Start is called before the first frame update
    public void CompleteLevelPopUp()
    {
        levelCompleteWindow.SetActive(true);
    }
}
