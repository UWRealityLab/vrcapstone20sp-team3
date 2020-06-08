using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

namespace Blockly {

public class PopupMessage : MonoBehaviour
{
  public static PopupMessage Instance = null;

  [NotNull]
    public GameObject ui;
    
  void Awake() {
    Debug.Assert(Instance == null, "singleton class instantiated multiple times");
    Instance = this;
  }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ClickButton();
        }

    }

    public void Open(string title, string instruction)
    {
        ui.SetActive(true);
        
/*        if (ui.activeSelf)
        {*/
            setText(title, 0);
            setText(instruction, 1);
/*        }*/
    }

    public void Close()
    {
        ui.SetActive(false);
        LevelController.Instance.SetupLevel();
        
    }

    public void ClickButton()
    {
        Button okButton = GameObject.Find("OK").GetComponent<Button>();
        okButton.onClick.Invoke();
    }

        private void setText(string message, int childNum)
    {
        if (message != null)
        {
            Text[] textObject = ui.gameObject.GetComponentsInChildren<Text>();
            textObject[childNum].text = message;
        }
    }
}

}
