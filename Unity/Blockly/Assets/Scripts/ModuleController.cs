using Blockly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Blockly {

public class ModuleController : MonoBehaviour
{
    public static ModuleController Instance = null;

    private bool isRecordingModule;
    private Module currentModule;
    private List<Module> allModules;  // list of modules (which are lists of actions)

    /* cursor-related, module creation/recording */
    public GameObject cursor;
    private Vector3 originalCursorPosition;  // for resetting cursor after completing module recording

    public Material blockMaterial;

    /* module library */
    public GameObject libraryBlockPrefab;
    public GameObject libraryModuleEndCursorPrefab;
    private GameObject selectedModule;  // currently selected module (out of the module library)
    private Dictionary<GameObject, int> objectToId;  // module library block -> index of module in allModules
    private Dictionary<int, Vector3> moduleLibraryPositions;  // module in library -> x, y, z position in library
    private const float LIBRARY_GRID_SIZE = CursorController.GRID_SIZE;  // size of blocks in module library

    void Awake() {
        Debug.Assert(Instance == null, "singleton class instantiated multiple times");
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.isRecordingModule = false;
        this.allModules = new List<Module>();
        this.objectToId = new Dictionary<GameObject, int>();
        this.moduleLibraryPositions = new Dictionary<int, Vector3>();
    }

    // Update is called once per frame
    // all the hardcoded keypress stuff is here for testing in play mode without gestures
    void Update()
    {
        /* SELECT - C */
        // Set the module near the player to be the currently selected module
        Select();

        /* APPLY - Z */
        // Apply the selected module to the main area based on given input
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (this.selectedModule != null)
            {
                Debug.Log("calling on Use Module with " + objectToId[this.selectedModule]);
                OnUseModule(objectToId[this.selectedModule]);
            }
            else
            {
                Debug.Log("no module was selected :(");
            }
        }

        /* LOOP - 4 to 9*/
        // Apply module however many times the input suggests
        // currently valid from 4 - 9 (inclusive) only
        // TODO: currently, 1 2 and 3 are used for emitting and moving, so 1-3 cannot be used
        // TODO: do we want them to be able to loop more than 10 times?
        for (int i = 4; i < 10; i++)
        {
            if (Input.GetKeyDown("" + i))
            {
                Debug.Log("looping " + i + " times");
                if (this.selectedModule != null)
                {
                    OnLoop(i, this.selectedModule);
                }
                break;
            }
        }

        /* DELETE - Y */
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Destroy(this.selectedModule);
        }
    }

    // Set the module to be the currently selected module
    private void Select()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            GameObject hand = GameObject.FindGameObjectWithTag("isHand");
            Transform handTransform = hand.transform;
            Vector3 handPosition = handTransform.position;
            Debug.Log("select module - hand position: " + handPosition);

            Collider[] hitColliders = Physics.OverlapSphere(handPosition, 5);
            foreach (Collider collider in hitColliders)
            {
                if (collider.gameObject.tag == "Library Block")
                {
                    this.selectedModule = collider.gameObject;
                    Debug.Log("module was selected!");
                    break;
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        // reset opacity of blocks in case user quits in middle of recording module
        setBlockMaterialTransparency(1f);
    }

    // start or stop recording a module
    public void OnPressRecord()
    {
        if (this.isRecordingModule)
        {
            this.OnEndModule();
        }
        else
        {
            this.OnBeginModule();
        }
    }

    private void OnBeginModule()
    {
        this.isRecordingModule = true;
        this.currentModule = new Module();
        this.originalCursorPosition = CursorController.Instance.gameObject.transform.position;
        setBlockMaterialTransparency(0.1f);
    }

    private void OnEndModule()
    {
        if (this.currentModule.Statements().Contains("Emit"))  // only store if module has blocks
        {
            this.currentModule.Complete();
            int moduleId = this.allModules.Count;
            this.allModules.Add(this.currentModule);
            this.AddToLibrary(moduleId);

            string result = "";
            foreach (var item in this.currentModule.Statements())
            {
                result += item.ToString() + ", ";
            }
            Debug.Log("recorded module #" + moduleId + ": " + result);
        }

        // delete the temporary blocks that were created when recording the module
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Module Creation Block");
        foreach (GameObject block in blocks)
        {
            Destroy(block);
        }

        this.isRecordingModule = false;
        CursorController.Instance.gameObject.transform.position = this.originalCursorPosition;
        this.currentModule = null;
        setBlockMaterialTransparency(1f);
    }

    // apply the given module to the main area
    public void OnUseModule(GameObject module)
    {
        this.OnUseModule(this.objectToId[module]);
    }

    // apply the module indicated by the given moduleId to the main area
    public void OnUseModule(int moduleId)
    {
        Module module = this.allModules[moduleId];
        Vector3 cursorPos = CursorController.Instance.CursorPosition();
        Vector3 minPos = module.MinPositionFromStart(cursorPos);
        Vector3 maxPos = module.MaxPositionFromStart(cursorPos);
        if (minPos.x < CursorController.MIN_POSITION
            || minPos.y < CursorController.MIN_POSITION
            || minPos.z < CursorController.MIN_POSITION
            || maxPos.x > CursorController.MAX_POSITION
            || maxPos.y > CursorController.MAX_POSITION
            || maxPos.z > CursorController.MAX_POSITION)
        {
            Debug.Log("module can't be applied (would go out of bounds)");
            // TODO: add error sound effect?
            return;
        }

        foreach (string statement in module.Statements())
        {
            Debug.Log("recognizing gesture");
            CursorController.Instance.OnRecognizeGesture(statement);
        }
    }

    // Apply the selected module specified number of times
    public void OnLoop(int times, GameObject module)
    {
        for (int i = 0; i < times; i++)
        {
            Debug.Log("looping " + i);
            OnUseModule(objectToId[module]);
            //cursorController.MoveRight();
        }
    }

    // add the given statement (action, e.g. "Emit") to the module that is currently recording
    public void AddStatement(string statement)
    {
        if (this.IsRecording())
        {
            this.currentModule.AddStatement(statement);
        }
    }

    // return true if there is a module currently being recorded
    public bool IsRecording()
    {
        return this.isRecordingModule;
    }

    // set transparency of all the current blocks to given alpha (0 is transparent, 1 is opaque)
    private void setBlockMaterialTransparency(float alpha)
    {
        Color fadedColor = this.blockMaterial.color;
        fadedColor.a = alpha;
        this.blockMaterial.color = fadedColor;
    }

    // add given module to the module library: draw a copy of the module in the module library
    // and add blocks to mapping of block->id
    private void AddToLibrary(int moduleId)
    {
        Vector3 libraryCursorPosition = this.ModuleIdToLibraryPosition(moduleId);
        Debug.Log("AddToLibrary: module #" + moduleId + " at " + libraryCursorPosition + "!");
        Module module = this.allModules[moduleId];

        GameObject moduleMeshObj = new GameObject("ModuleMesh");
        objectToId.Add(moduleMeshObj, moduleId);

        HashSet<Vector3> blockPositions = new HashSet<Vector3>();  // set containing positions where blocks exist
        foreach (string statement in module.Statements())
        {
            switch (statement)
            {
                case "Emit":
                    bool blockExisted = false;
                    Collider[] colliders = Physics.OverlapSphere(libraryCursorPosition, CursorController.GRID_SIZE / 4);

                    foreach (Collider collider in colliders)
                    {
                        if (collider.gameObject.tag == "Library Block")
                        {
                            Vector3 cursorCopy = new Vector3(libraryCursorPosition.x, libraryCursorPosition.y, libraryCursorPosition.z);
                            if (blockPositions.Contains(cursorCopy))
                            {
                                Debug.Log("adding to library: emit: delete obj @ " + collider.gameObject.transform.position);
                                blockExisted = true;
                                Destroy(collider.gameObject);
                                blockPositions.Remove(cursorCopy);

                            }
                            break;
                        }
                    }

                    if (!blockExisted)
                    {
                        Debug.Log("adding to library: emit: actually emit @ " + libraryCursorPosition);
                        GameObject obj = Instantiate(this.libraryBlockPrefab, libraryCursorPosition, Quaternion.identity);
                        obj.transform.parent = moduleMeshObj.transform;
                        objectToId.Add(obj, moduleId);
                        blockPositions.Add(new Vector3(libraryCursorPosition.x, libraryCursorPosition.y, libraryCursorPosition.z));
                    }
                    break;
                case "Right":
                    libraryCursorPosition.x += LIBRARY_GRID_SIZE;
                    break;
                case "Left":
                    libraryCursorPosition.x -= LIBRARY_GRID_SIZE;
                    break;
                case "Up":
                    libraryCursorPosition.y += LIBRARY_GRID_SIZE;
                    break;
                case "Down":
                    libraryCursorPosition.y -= LIBRARY_GRID_SIZE;
                    break;
                case "Forward":
                    libraryCursorPosition.z += LIBRARY_GRID_SIZE;
                    break;
                case "Backward":
                    libraryCursorPosition.z -= LIBRARY_GRID_SIZE;
                    break;
                default:
                    Debug.Log("unrecognized statement in module #" + moduleId + ": " + statement);
                    break;
            }
        }
        GameObject endCursor = Instantiate(this.libraryModuleEndCursorPrefab, libraryCursorPosition, Quaternion.identity);
        endCursor.transform.parent = moduleMeshObj.transform;
        objectToId.Add(endCursor, moduleId);

        ModuleLibrary.Instance.AddModule(moduleId, moduleMeshObj);
    }

    private Vector3 ModuleIdToLibraryPosition(int moduleId)
    {
        Module module = this.allModules[moduleId];
        Vector3 minCorner = new Vector3(-20.5f, 2f, -1f);  // min corner of entire library area

        if (moduleId != 0)  // if not first module, base position on preceding module
        {
            Module prevModule = this.allModules[moduleId - 1];
            Vector3 prevStart = this.moduleLibraryPositions[moduleId - 1];
            Vector3 prevMax = prevModule.MaxPositionFromStart(prevStart);
            Vector3 prevMin = prevModule.MinPositionFromStart(prevStart);

            if (prevMax.x > -9.5f - 1 - prevModule.TotalSize().x)  // need to go to next row
            {
                minCorner.x = -20.5f;
                minCorner.z = prevMin.z + 10f;
            }
            else
            {
                minCorner.x = prevMax.x + 2f;
                minCorner.z = prevMin.z;
            }
        }

        this.moduleLibraryPositions[moduleId] = module.StartPositionFromMinCorner(minCorner);
        return this.moduleLibraryPositions[moduleId];
    }

    // return true if there are no modules in the library
    public Boolean ModuleLibraryIsEmpty()
    {
        return this.allModules.Count == 0;
    }

    // returns the position in the library for the most recently saved module
    // returns zero vector if there are no saved modules
    public Vector3 MostRecentModuleLibraryPosition()
    {
        if (this.allModules.Count == 0)
        {
            return Vector3.zero;
        }
        return this.moduleLibraryPositions[this.allModules.Count - 1];
    }
}

}
