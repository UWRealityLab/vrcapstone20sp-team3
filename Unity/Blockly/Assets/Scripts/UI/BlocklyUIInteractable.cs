using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using OculusSampleFramework;

namespace Blockly
{

public class BlocklyUIInteractable : MonoBehaviour {
  [SerializeField]
  private UnityEvent onButtonPinch;

  [SerializeField] [NotNull]
  private GameObject _startStopButton = null;
  [SerializeField] [NotNull]
  private SelectionCylinder _selectionCylinder = null;

	private InteractableTool _toolInteractingWithMe = null;

  private void OnEnable()
  {
    _startStopButton.GetComponent<Interactable>().InteractableStateChanged.AddListener(StartStopStateChanged);
  }

  private void OnDisable()
  {
    if (_startStopButton != null)
    {
      _startStopButton.GetComponent<Interactable>().InteractableStateChanged.RemoveListener(StartStopStateChanged);
    }
  }

  public void Update() {
    if (_toolInteractingWithMe == null) return;

    if (_toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDown || _toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDownStay) {
        // finger is pinching
        onButtonPinch.Invoke();
    }
  }

  private void StartStopStateChanged(InteractableStateArgs obj)
  {
    if (obj.Tool == null) return;

    if (_toolInteractingWithMe == null) {
      _toolInteractingWithMe = obj.Tool;
    }

    if (obj.Tool == _toolInteractingWithMe) {
      if (obj.NewInteractableState > InteractableState.Default) {
        _selectionCylinder.CurrSelectionState = SelectionCylinder.SelectionState.Selected;
      } else if (obj.NewInteractableState == InteractableState.Default) {
        _toolInteractingWithMe = null;
      }
    }

    if (_toolInteractingWithMe == null) {
      // tool stopped interacting with us
      _selectionCylinder.CurrSelectionState = SelectionCylinder.SelectionState.Off;

      if (obj.NewInteractableState > InteractableState.Default) {
        // a new object is interacting with us
        _toolInteractingWithMe = obj.Tool;
      }
    }
  }
}

}
