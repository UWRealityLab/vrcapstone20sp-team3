using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    GameObject button;

    // Start is called before the first frame update
    private void Start()
    {
        button = GameObject.Find("Button");
    }
    public void BeginAdventure()
    {
        SceneManager.LoadScene("BlockArea", LoadSceneMode.Additive);
        SceneManager.LoadScene("GestureRecognition", LoadSceneMode.Additive);
        button.SetActive(false);
    }
}
