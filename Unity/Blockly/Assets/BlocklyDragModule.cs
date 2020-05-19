using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using OculusSampleFramework;

namespace Blockly {

public class BlocklyDragModule : MonoBehaviour {
    [SerializeField] private GameObject _startStopButton = null;
    [SerializeField] private BoxCollider _boxCollider = null;
    [SerializeField] float _maxSpeed = 10f;

    public InteractableTool toolInteractingWithMe = null;
    public int moduleName;

    private Renderer moduleRenderer;
    private DragModulePlayAreaChecker playAreaChecker;

    // TODO idea for loop recognition: have one hand holding the module and draw
    // loops with the other hand (count loops by the number of times the other
    // hand enters proximity and exits)

    public void Awake()
    {
        Assert.IsNotNull(_startStopButton);
        Assert.IsNotNull(_boxCollider);
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Debug.Assert(renderers.Length == 1);
        moduleRenderer = renderers[0];
        playAreaChecker = GetComponentInChildren<DragModulePlayAreaChecker>();
    }

    public void Start() {
        Assert.IsNotNull(toolInteractingWithMe);
        transform.position = CalcToolEnd();
    }

    public void Update()
    {
        if (toolInteractingWithMe == null) return;

        if (toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDown || toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDownStay) {
            moduleRenderer.material.color = Color.blue;
            transform.position = Vector3.Lerp(transform.position, CalcToolEnd(), 0.2f);
        } else {
            moduleRenderer.material.color = Color.red;
            toolInteractingWithMe.DeFocus();
            toolInteractingWithMe = null;

            if (playAreaChecker.InPlayArea()) {
                Debug.Log($"doobs: placed module {moduleName}");
            } else {
                Debug.Log($"doobs: did NOT place module {moduleName}");
            }

            _startStopButton.SetActive(false);
            gameObject.SetActive(false);
            // TODO oculus code breaks when trying to delete interactables, so we just
            // move it REALLY far away
            transform.position.Set(0f, -100f, 0f);
        }
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
        if (toolInteractingWithMe == null || obj.Tool == toolInteractingWithMe) {
            if (obj.Tool.ToolInputState == ToolInputState.PrimaryInputDown || obj.Tool.ToolInputState == ToolInputState.PrimaryInputDownStay) {
                toolInteractingWithMe = obj.Tool;
            }
        }
    }

    private Vector3 CalcToolEnd() {
        Transform toolTransform = toolInteractingWithMe.ToolTransform;
        return toolTransform.position + toolTransform.forward * 2f;
    }
}

}
