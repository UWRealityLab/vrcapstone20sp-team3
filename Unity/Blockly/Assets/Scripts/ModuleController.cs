using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleController : MonoBehaviour
{
    private bool isRecordingModule;
    private List<Statement> currentModule;
    private Dictionary<string, List<Statement>> allModules;

    public CursorController cursorController;

    // Start is called before the first frame update
    void Start()
    {
        this.isRecordingModule = false;
        this.allModules = new Dictionary<string, List<Statement>>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // TODO: if record button is clicked, call OnBeginModule() / OnEndModule() appropriately
    // TODO: figure out how the UI should look for using modules & call OnUseModule()

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
