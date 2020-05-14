using UnityEngine;
using UnityEngine.Assertions;
using OculusSampleFramework;

namespace Blockly
{

public class BlocklyModuleUIController : MonoBehaviour
{
	[SerializeField] private GameObject _startStopButton = null;
	[SerializeField] float _maxSpeed = 10f;
	[SerializeField] private SelectionCylinder _selectionCylinder = null;

	[SerializeField]
	private GameObject _moduleMeshObj;
	// private WindmillBladesController _bladesRotation;
	private InteractableTool _toolInteractingWithMe = null;

	private void Awake()
	{
		Assert.IsNotNull(_startStopButton);
		Assert.IsNotNull(_selectionCylinder);
	}

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

	private void StartStopStateChanged(InteractableStateArgs obj)
	{
		// bool inActionState = obj.NewInteractableState == InteractableState.ActionState;
		// if (inActionState)
		// {
		// 	if (_moduleMeshObj.GetComponent<Renderer>().material.color == Color.blue) {
		// 		_moduleMeshObj.GetComponent<Renderer>().material.color = Color.white;
		// 	} else {
		// 		_moduleMeshObj.GetComponent<Renderer>().material.color = Color.blue;
		// 	}
		// }
		if (obj.Tool == null) return;


		if (_toolInteractingWithMe == null) {
			_toolInteractingWithMe = obj.Tool;
		}

		if (obj.Tool == _toolInteractingWithMe) {
			if (obj.NewInteractableState > InteractableState.Default) {
				_selectionCylinder.CurrSelectionState = SelectionCylinder.SelectionState.Selected;
			}

			if (obj.Tool.ToolInputState == ToolInputState.PrimaryInputDown || obj.Tool.ToolInputState == ToolInputState.PrimaryInputDownStay) {
				// finger is pinching
				if ((obj.OldInteractableState == InteractableState.ProximityState || obj.OldInteractableState == InteractableState.ContactState)
					&& obj.NewInteractableState == InteractableState.Default) {
					// ray went outside of proximity zone while pinching (i.e., the module was dragged out of the library)
					_moduleMeshObj.GetComponent<Renderer>().material.color = Color.red;
					// TODO once we have the machinery to make a copy of the
					// selected module that follows the ray, set the selection
					// state to off
					// _selectionCylinder.CurrSelectionState = SelectionCylinder.SelectionState.Off;
				} else {
					_selectionCylinder.CurrSelectionState = SelectionCylinder.SelectionState.Highlighted;
					_moduleMeshObj.GetComponent<Renderer>().material.color = Color.cyan;
				}
			} else if (obj.Tool.ToolInputState == ToolInputState.PrimaryInputUp || obj.Tool.ToolInputState == ToolInputState.Inactive) {
				// finger is not pinching
				if (obj.NewInteractableState == InteractableState.Default) {
					_toolInteractingWithMe = null;
				}
			}
		}

		if (_toolInteractingWithMe == null) {
			// tool stopped interacting with us
			_moduleMeshObj.GetComponent<Renderer>().material.color = Color.white;
			_selectionCylinder.CurrSelectionState = SelectionCylinder.SelectionState.Off;

			if (obj.NewInteractableState > InteractableState.Default) {
				// a new object is interacting with us
				_toolInteractingWithMe = obj.Tool;
			}
		}
	}
}

}
