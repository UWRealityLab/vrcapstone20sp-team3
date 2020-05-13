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
    public GameObject libraryBlockPrefab;
    private GameObject selectedModule;  // currently selected module (out of the module library)
    private Dictionary<GameObject, int> objectToName;  // module library block -> index of module in allModules
    private const int FIRST_ROW_OFFSET = -15;  // x-value of first row of module library
    private const int ROW_LENGTH = 5;  // number of modules in one row of the module library
    private const float LIBRARY_GRID_SIZE = 1f;  // size of blocks in module library

    // Start is called before the first frame update
    void Start()
    {
        this.cursorController = cursor.GetComponent<CursorController>();
        this.isRecordingModule = false;
        this.allModules = new List<List<Statement>>();
        this.objectToName = new Dictionary<GameObject, int>();
    }

    // Update is called once per frame
    void Update()
    {
        Select();
        ApplyModule();
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
            int moduleName = this.allModules.Count;
            this.allModules.Add(this.currentModule);
            this.AddToLibrary(moduleName);

            string result = "";
            foreach (var item in this.currentModule)
            {
                result += item.ToString() + ", ";
            }
            Debug.Log("recorded module #" + moduleName + ": " + result);
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

    public void OnUseModule(int moduleName)
    {
        List<Statement> module = this.allModules[moduleName];
        foreach (Statement statement in module)
        {
            if (statement.isGesture)
            {
                Debug.Log("recognizing gesture");
                cursorController.OnRecognizeGesture(statement.name);
            }
/*            else
            {
                // recursive apply submodule
                this.OnUseModule(statement.name);
            }*/
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

    // if module is clicked on, set the module to be the currently selected module
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


        //Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        //int i = 0;
        //while (i < hitColliders.Length)
        //{
        //    hitColliders[i].SendMessage("AddDamage");
        //    i++;
        //}

        //if (Input.GetMouseButtonDown(0))
        //{
        //    Debug.Log("Mouse is down");
        //    RaycastHit hitInfo = new RaycastHit();
        //    bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
        //    if (hit)
        //    {
        //        Debug.Log("Hit " + hitInfo.transform.gameObject.name);
        //        if (hitInfo.collider.gameObject.tag == "isModule")
        //        {
        //            this.selectedModule = hitInfo.collider.gameObject;
        //            Debug.Log("module was successfully selected");
        //        }
        //        else
        //        {
        //            Debug.Log("not a module");
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log("No hit");
        //    }
        //}
    }

    private void ApplyModule()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (this.selectedModule != null)
            {
                Debug.Log("calling on Use Module with " + objectToName[this.selectedModule]);
                OnUseModule(objectToName[this.selectedModule]);
            } else
            {
                Debug.Log("no module was selected :(");
            }
        }
    }

    // add given module to the module library: draw a copy of the module in the module library
    // and add blocks to mapping of block->name
    private void AddToLibrary(int moduleName)
    {
        Vector3 startPosition = this.moduleNameToLibraryPosition(moduleName);
        Debug.Log("AddToLibrary: module #" + moduleName + " at " + startPosition + "!");
        List<Statement> module = this.allModules[moduleName];
        foreach (Statement statement in module)
        {
            switch (statement.name)
            {
                case "Emit":
                    GameObject obj = Instantiate(this.libraryBlockPrefab, startPosition, Quaternion.identity);
                    objectToName.Add(obj, moduleName);
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
                    Debug.Log("unrecognized statement in module #" + moduleName + ": " + statement.name);
                    break;
            }
        }
    }

    private Vector3 moduleNameToLibraryPosition(int moduleName)
    {
        float x = FIRST_ROW_OFFSET +- moduleName / 5;
        float z = moduleName % ROW_LENGTH;
        return new Vector3(x, 2f, z * 11);
    }
}
