using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleController : MonoBehaviour
{
    private bool isRecordingModule;
    private List<Statement> currentModule;
    private Dictionary<string, List<Statement>> allModules;
    private Dictionary<GameObject, string> objectToName;

    public GameObject cursor;
    private CursorController cursorController;
    private Vector3 originalCursorPosition;  // for resetting cursor after completing module recording

    public Material blockMaterial;
    private GameObject selectedModule;

    // Start is called before the first frame update
    void Start()
    {
        this.cursorController = cursor.GetComponent<CursorController>();
        this.isRecordingModule = false;
        this.allModules = new Dictionary<string, List<Statement>>();
        this.objectToName = new Dictionary<GameObject, string>();
    }

    // Update is called once per frame
    void Update()
    {

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
            string moduleName = "module" + this.allModules.Count;
            this.allModules.Add(moduleName, this.currentModule);

            string result = "";
            foreach (var item in this.currentModule)
            {
                result += item.ToString() + ", ";
            }
            Debug.Log("recorded module [" + moduleName + "]: " + result);
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

    public void OnUseModule(string moduleName)
    {
        List<Statement> module = this.allModules[moduleName];
        foreach (Statement statement in module)
        {
            if (statement.isGesture)
            {
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
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse is down");
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {
                Debug.Log("Hit " + hitInfo.transform.gameObject.name);
                if (hitInfo.collider.gameObject.tag == "isModule")
                {
                    this.selectedModule = hitInfo.collider.gameObject;
                    Debug.Log("module was successfully selected");
                }
                else
                {
                    Debug.Log("not a module");
                }
            }
            else
            {
                Debug.Log("No hit");
            }
        }
    }

    private void ApplyModule()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            OnUseModule(objectToName[this.selectedModule]);
        }
    }
}
