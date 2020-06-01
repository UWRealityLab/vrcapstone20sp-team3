using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Blockly {

// Show off all the Debug UI components.
public class BlocklyUISample : MonoBehaviour
{
    bool inMenu;
    private Text sliderText;

	void Start ()
    {
        BlocklyUIBuilder.instance.AddButton("Button Pressed", LogButtonPressed);
        BlocklyUIBuilder.instance.AddLabel("Label");
        var sliderPrefab = BlocklyUIBuilder.instance.AddSlider("Slider", 1.0f, 10.0f, SliderPressed, true);
        var textElementsInSlider = sliderPrefab.GetComponentsInChildren<Text>();
        Assert.AreEqual(textElementsInSlider.Length, 2, "Slider prefab format requires 2 text components (label + value)");
        sliderText = textElementsInSlider[1];
        Assert.IsNotNull(sliderText, "No text component on slider prefab");
        sliderText.text = sliderPrefab.GetComponentInChildren<Slider>().value.ToString();
        BlocklyUIBuilder.instance.AddDivider();
        BlocklyUIBuilder.instance.AddToggle("Toggle", TogglePressed);
        BlocklyUIBuilder.instance.AddRadio("Radio1", "group", delegate(Toggle t) { RadioPressed("Radio1", "group", t); }) ;
        BlocklyUIBuilder.instance.AddRadio("Radio2", "group", delegate(Toggle t) { RadioPressed("Radio2", "group", t); }) ;
        BlocklyUIBuilder.instance.AddLabel("Secondary Tab", 1);
		BlocklyUIBuilder.instance.AddDivider(1);
        BlocklyUIBuilder.instance.AddRadio("Side Radio 1", "group2", delegate(Toggle t) { RadioPressed("Side Radio 1", "group2", t); }, DebugUIBuilder.DEBUG_PANE_RIGHT);
        BlocklyUIBuilder.instance.AddRadio("Side Radio 2", "group2", delegate(Toggle t) { RadioPressed("Side Radio 2", "group2", t); }, DebugUIBuilder.DEBUG_PANE_RIGHT);

        BlocklyUIBuilder.instance.Show();
        inMenu = true;
	}

    public void TogglePressed(Toggle t)
    {
        Debug.Log("Toggle pressed. Is on? "+t.isOn);
    }
    public void RadioPressed(string radioLabel, string group, Toggle t)
    {
        Debug.Log("Radio value changed: "+radioLabel+", from group "+group+". New value: "+t.isOn);
    }

    public void SliderPressed(float f)
    {
        Debug.Log("Slider: " + f);
        sliderText.text = f.ToString();
    }

    void Update()
    {
        if(OVRInput.GetDown(OVRInput.Button.Two) || OVRInput.GetDown(OVRInput.Button.Start))
        {
            if (inMenu) BlocklyUIBuilder.instance.Hide();
            else BlocklyUIBuilder.instance.Show();
            inMenu = !inMenu;
        }
    }

    void LogButtonPressed()
    {
        Debug.Log("Button pressed");
    }
}

}
