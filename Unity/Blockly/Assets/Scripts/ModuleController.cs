using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleController : MonoBehaviour
{
    private bool isRecordingModule;
    private List<Statement> currentModule;
    private Dictionary<string, List<Statement>> allModules;

    public GameObject cursor;
    private CursorController cursorController;

    // Start is called before the first frame update
    void Start()
    {
        this.cursorController = cursor.GetComponent<CursorController>();
        this.isRecordingModule = false;
        this.allModules = new Dictionary<string, List<Statement>>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
    }

    public void OnEndModule()
    {
        this.isRecordingModule = false;
        string moduleName = "module" + this.allModules.Count;
        this.allModules.Add(moduleName, this.currentModule);
        string result = "";
        foreach (var item in this.currentModule)
        {
            result += item.ToString() + ", ";
        }
        Debug.Log("recorded module [" + moduleName + "]: " + result);
        this.currentModule = null;
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
            else
            {
                // recursive apply submodule
                this.OnUseModule(statement.name);
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
}
