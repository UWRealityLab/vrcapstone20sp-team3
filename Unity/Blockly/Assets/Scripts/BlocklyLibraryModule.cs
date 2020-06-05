using UnityEngine;
using UnityEngine.Assertions;
using OculusSampleFramework;

namespace Blockly
{

public class BlocklyLibraryModule : MonoBehaviour
{
	[SerializeField] [NotNull]
	private GameObject _startStopButton = null;
	[SerializeField] [NotNull]
	private SelectionCylinder _selectionCylinder = null;

	[SerializeField] [NotNull]
	private GameObject _baseMeshObj;
	[SerializeField] [NotNull]
	private GameObject _dragModulePrefab;

	public int moduleName = -1;

	private InteractableTool _toolInteractingWithMe = null;

	private void Awake()
	{
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
					_baseMeshObj.GetComponent<Renderer>().material.color = Color.red;
					_selectionCylinder.CurrSelectionState = SelectionCylinder.SelectionState.Off;
					GameObject dragModObj = Instantiate(_dragModulePrefab);
					dragModObj.transform.position = transform.position;
					BlocklyDragModule dragMod = dragModObj.GetComponent<BlocklyDragModule>();
					dragMod.toolInteractingWithMe = obj.Tool;
					dragMod.moduleName = moduleName;
					Debug.Log($"creating drag module with moduleName={moduleName}");

					GameObject moduleMeshObj = _baseMeshObj.transform.Find("ModuleMesh").gameObject;
					GameObject moduleMeshObjCopy = Instantiate(moduleMeshObj);
					moduleMeshObjCopy.transform.parent = dragModObj.transform.Find("Mesh");
					moduleMeshObjCopy.transform.localPosition = Vector3.zero;
					moduleMeshObjCopy.transform.localScale = moduleMeshObj.transform.localScale;

					Debug.Log($"finished creating drag module");

					_toolInteractingWithMe = null;
				} else {
					_selectionCylinder.CurrSelectionState = SelectionCylinder.SelectionState.Highlighted;
					_baseMeshObj.GetComponent<Renderer>().material.color = Color.cyan;
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
			_baseMeshObj.GetComponent<Renderer>().material.color = Color.white;
			_selectionCylinder.CurrSelectionState = SelectionCylinder.SelectionState.Off;

			if (obj.NewInteractableState > InteractableState.Default) {
				// a new object is interacting with us
				_toolInteractingWithMe = obj.Tool;
			}
		}
	}
}

}
