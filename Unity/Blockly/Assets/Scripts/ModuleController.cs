using Blockly;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleController : MonoBehaviour
{
    private bool isRecordingModule;
    private List<Statement> currentModule;
    private List<List<Statement>> allModules;  // list of modules (which are lists of actions)

    /* cursor-related, module creation/recording */
    public GameObject cursor;
    private CursorController cursorController;
    private Vector3 originalCursorPosition;  // for resetting cursor after completing module recording

    public Material blockMaterial;

    /* module library */
    public GameObject libraryModuleParentPrefab;
    public GameObject libraryBlockPrefab;
    private GameObject selectedModule;  // currently selected module (out of the module library)
    private Dictionary<GameObject, int> objectToId;  // module library block -> index of module in allModules
    private const int FIRST_ROW_OFFSET = -15;  // x-value of first row of module library
    private const int ROW_LENGTH = 5;  // number of modules in one row of the module library
    private const float LIBRARY_GRID_SIZE = 1f;  // size of blocks in module library

    // Start is called before the first frame update
    void Start()
    {
        this.cursorController = cursor.GetComponent<CursorController>();
        this.isRecordingModule = false;
        this.allModules = new List<List<Statement>>();
        this.objectToId = new Dictionary<GameObject, int>();
    }

    // Update is called once per frame
    void Update()
    {
        Select();
        ApplyModule();
        LoopModule();
        DeleteModule();
    }

    void OnApplicationQuit()
    {
        // reset opacity of blocks in case user quits in middle of recording module
        setBlockMaterialTransparency(1f);
    }

    // TODO: if record button is clicked, call OnBeginModule() / OnEndModule() appropriately
    // TODO: figure out how the UI should look for using modules & call OnUseModule()

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

    public void OnBeginModule()
    {
        this.isRecordingModule = true;
        this.currentModule = new List<Statement>();
        this.originalCursorPosition = this.cursorController.gameObject.transform.position;
        setBlockMaterialTransparency(0.1f);
    }

    public void OnEndModule()
    {
        if (this.currentModule.Count > 0)  // only store if module has statements
        {
            int moduleId = this.allModules.Count;
            this.allModules.Add(this.currentModule);
            this.AddToLibrary(moduleId);

            string result = "";
            foreach (var item in this.currentModule)
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
        this.cursorController.gameObject.transform.position = this.originalCursorPosition;
        this.currentModule = null;
        setBlockMaterialTransparency(1f);
    }

    public void OnUseModule(GameObject module)
    {
        this.OnUseModule(this.objectToId[module]);
    }

    public void OnUseModule(int moduleId)
    {
        List<Statement> module = this.allModules[moduleId];
        foreach (Statement statement in module)
        {
            if (statement.isGesture)
            {
                Debug.Log("recognizing gesture");
                cursorController.OnRecognizeGesture(statement.name);
            }
            else if (statement.ToString().Equals("Loop"))
            {
                this.selectedModule = statement.module;
                Loop(statement.times);                
            }
        }
    }

    public void AddStatement(Statement statement)
    {
        this.currentModule.Add(statement);
    }

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

    // Set the module to be the currently selected module
    private void Select()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            GameObject hand = GameObject.FindGameObjectWithTag("isHand");
            Transform handTransform = hand.transform;
            Vector3 handPosition = handTransform.position;
            Debug.Log("hand position: " + handPosition);

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

    // Apply the selected module to the main area based on given input
    private void ApplyModule()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Apply();
        }
    }

    // Apply the selected module once
    private void Apply()
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

    // Apply module however many times the input suggests
    // currently valid from 4 - 9 (inclusive) only
    // TODO: currently, 1 2 and 3 are used for emitting and moving, so 1-3 cannot be used
    // TODO: do we want them to be able to loop more than 10 times?
    private void LoopModule()
    {
        for (int i = 4; i < 10; i++)
        {
            if (Input.GetKeyDown("" + i))
            {
                Debug.Log("looping " + i + " times");
                Loop(i);
                break;
            }
        }
    }

    // Apply the selected module specified number of times
    private void Loop(int times)
    {
        for (int i = 0; i < times; i++)
        {
            Debug.Log("looping " + i);
            Apply();
            //cursorController.MoveRight();
        }

        if (IsRecording())
        {
            Statement s = new Statement("Loop", false, this.selectedModule, times);
            AddStatement(s);
        }
    }

    private void DeleteModule()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Destroy(this.selectedModule);
        }
    }

    // add given module to the module library: draw a copy of the module in the module library
    // and add blocks to mapping of block->id
    private void AddToLibrary(int moduleId)
    {
        Vector3 startPosition = this.moduleIdToLibraryPosition(moduleId);
        Debug.Log("AddToLibrary: module #" + moduleId + " at " + startPosition + "!");
        List<Statement> module = this.allModules[moduleId];

        GameObject parentObject = new GameObject();
        // GameObject parentObject = Instantiate(this.libraryModuleParentPrefab, startPosition, Quaternion.identity) as GameObject;

        objectToId.Add(parentObject, moduleId);
        foreach (Statement statement in module)
        {
            switch (statement.name)
            {
                case "Emit":
                    GameObject obj = Instantiate(this.libraryBlockPrefab, startPosition, Quaternion.identity);
                    obj.transform.parent = parentObject.transform;
                    objectToId.Add(obj, moduleId);
                    break;
                // case "Delete":
                //     break;
                case "Right":
                    startPosition.x += LIBRARY_GRID_SIZE;
                    break;
                case "Left":
                    startPosition.x -= LIBRARY_GRID_SIZE;
                    break;
                case "Up":
                    startPosition.y += LIBRARY_GRID_SIZE;
                    break;
                case "Down":
                    startPosition.y -= LIBRARY_GRID_SIZE;
                    break;
                case "Forward":
                    startPosition.z += LIBRARY_GRID_SIZE;
                    break;
                case "Backward":
                    startPosition.z -= LIBRARY_GRID_SIZE;
                    break;
                default:
                    Debug.Log("unrecognized statement in module #" + moduleId + ": " + statement.name);
                    break;
            }
        }
    }

    private Vector3 moduleIdToLibraryPosition(int moduleId)
    {
        float x = FIRST_ROW_OFFSET +-moduleId / 5;
        float z = moduleId % ROW_LENGTH;
        return new Vector3(x, 2f, z * 11);
    }
}
