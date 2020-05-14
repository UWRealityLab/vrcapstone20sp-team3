﻿using UnityEngine;
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

		// _bladesRotation = GetComponentInChildren<WindmillBladesController>();

		// _bladesRotation.SetMoveState(true, _maxSpeed);
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
		bool inActionState = obj.NewInteractableState == InteractableState.ActionState;
		if (inActionState)
		{
			if (_moduleMeshObj.GetComponent<Renderer>().material.color == Color.blue) {
				_moduleMeshObj.GetComponent<Renderer>().material.color = Color.white;
			} else {
				_moduleMeshObj.GetComponent<Renderer>().material.color = Color.blue;
			}
			// if (_bladesRotation.IsMoving)
			// {
			// 	_bladesRotation.SetMoveState(false, 0.0f);
			// }
			// else
			// {
			// 	_bladesRotation.SetMoveState(true, _maxSpeed);
			// }
		}

		_toolInteractingWithMe = obj.NewInteractableState > InteractableState.Default ?
			obj.Tool : null;
	}

	private void Update()
	{
		if (_toolInteractingWithMe == null)
		{
			_selectionCylinder.CurrSelectionState = SelectionCylinder.SelectionState.Off;
		}
		else
		{
			_selectionCylinder.CurrSelectionState = (
				_toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDown ||
				_toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDownStay)
				? SelectionCylinder.SelectionState.Highlighted
				: SelectionCylinder.SelectionState.Selected;
		}
	}
}

}
