using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using OculusSampleFramework;

namespace Blockly {

public class BlocklyDragModule : MonoBehaviour {
    [SerializeField] private GameObject _startStopButton = null;
    [SerializeField] float _maxSpeed = 10f;

    public InteractableTool toolInteractingWithMe = null;

    private Renderer moduleRenderer;

    // TODO try slowly uncommenting things in here to see what breaks.
    // TODO try just stoppping tracking the ray tool when no longer pinching
    // (maybe Destroy is breaking the code)
    // TODO idea for loop recognition: have one hand holding the module and draw
    // loops with the other hand (count loops by the number of times the other
    // hand enters proximity and exits)

    public void Awake()
    {
        Debug.Log("doobs: awake");
        Assert.IsNotNull(_startStopButton);
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Debug.Assert(renderers.Length == 1);
        moduleRenderer = renderers[0];
    }

    public void Start() {
        Assert.IsNotNull(toolInteractingWithMe);
        SetTool(toolInteractingWithMe);
    }

    public void Update()
    {
        if (toolInteractingWithMe == null) return;

        Debug.Log($"doobs: tool state == {toolInteractingWithMe.ToolInputState}");
        if (toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDown || toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDownStay) {
            moduleRenderer.material.color = Color.blue;
        } else {
            transform.position = Vector3.Lerp(transform.position, CalcToolEnd(), 2f);
            moduleRenderer.material.color = Color.red;
            // DetachTool();
        }
        transform.position = Vector3.Lerp(transform.position, CalcToolEnd(), 1f);
    }

    private void OnEnable()
    {
        Debug.Log("doobs: we enabled");
        Assert.IsNotNull(toolInteractingWithMe);
        _startStopButton.GetComponent<Interactable>().InteractableStateChanged.AddListener(StartStopStateChanged);
    }

    private void OnDisable()
    {
        Debug.Log("doobs: we disabled");
        if (_startStopButton != null)
        {
            _startStopButton.GetComponent<Interactable>().InteractableStateChanged.RemoveListener(StartStopStateChanged);
        }
    }

    private void StartStopStateChanged(InteractableStateArgs obj)
    {
        Debug.Log("doobs: state changed");
        // Debug.Assert(toolInteractingWithMe != null);
        // Debug.Assert(obj.Tool == toolInteractingWithMe);
        // Debug.Assert(obj.OldInteractableState > InteractableState.Default);

        if (toolInteractingWithMe == null || obj.Tool == toolInteractingWithMe) {
            if (obj.Tool.ToolInputState == ToolInputState.PrimaryInputDown || obj.Tool.ToolInputState == ToolInputState.PrimaryInputDownStay) {
                // moduleRenderer.material.color = Color.blue;
                SetTool(obj.Tool);
            } else {
                // moduleRenderer.material.color = Color.red;
                // DetachTool();
            }
        }
    }

    private void SetTool(InteractableTool tool) {
        toolInteractingWithMe = tool;
        gameObject.transform.parent = toolInteractingWithMe.ToolTransform;
        gameObject.transform.position = CalcToolEnd();
    }

    private Vector3 CalcToolEnd() {
        Transform toolTransform = toolInteractingWithMe.ToolTransform;
        return toolTransform.position + toolTransform.forward * 2f;
    }

    private void DetachTool() {
        toolInteractingWithMe = null;
        gameObject.transform.parent = null;
    }
}

}
