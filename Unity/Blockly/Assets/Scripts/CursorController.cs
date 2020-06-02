using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blockly {

public class CursorController : MonoBehaviour
{
    public static CursorController Instance = null;

    public GameObject blockPrefab;  // prefab for blocks, used when emitted
    public GameObject moduleCreationBlockPrefab;  // prefab for the temporary blocks that show up during module creation

    public GameObject recordButton;

    public AudioClip emitSound;
    public AudioClip moveSound;

    // number of cells along each axis of the grid
    public const int GRID_SIZE = 10;
    public const float CELL_SIZE = 1f / GRID_SIZE;
    public Vector3Int index;

    private AudioSource source;

    void Awake()
    {
        Debug.Assert(Instance == null, "singleton class instantiated multiple times");
        Instance = this;
        source = GetComponent<AudioSource>();
        index = Vector3Int.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void OnRecognizeGesture(string gestureName)
    {
        if (gestureName == "CreateModule")
        {
            ModuleController.Instance.OnPressRecord();
            return;
        }

        Vector3Int oldIndex = index;
        if (gestureName == "Emit")
        {
            Emit();
            source.PlayOneShot(emitSound);
        }
        else
        {
            source.PlayOneShot(moveSound);
            if (gestureName == "Left")
            {
                MoveLeft();
            }
            else if (gestureName == "Right")
            {
                MoveRight();
            }
            else if (gestureName == "Backward")
            {
                MoveBackward();
            }
            else if (gestureName == "Forward")
            {
                MoveForward();
            }
            else if (gestureName == "Up")
            {
                MoveUp();
            }
            else if (gestureName == "Down")
            {
                MoveDown();
            }
            UpdatePosition();
        }


        if (ModuleController.Instance.IsRecording())
        {
            // only record if the move actually happens (don't record if cursor is at edge of valid region and doesn't actually move)
            if (gestureName != "Emit" && index == oldIndex)
            {
                return;
            }
            ModuleController.Instance.AddStatement(gestureName);
        }

        // if (Input.GetKeyDown(KeyCode.Delete))
        // {
        //     Delete();
        // }
    }

    public void MoveRight()
    {
        if (index.x == GRID_SIZE - 1)
        {
            return;
        }
        index.x++;
        // Vector3 position = this.gameObject.transform.localPosition;
        // position.x += 1f;
        // this.gameObject.transform.localPosition = position;
    }

    public void MoveLeft()
    {
        if (index.x == 0)
        {
            return;
        }
        index.x--;
        // Vector3 position = this.gameObject.transform.localPosition;
        // position.x -= 1f;
        // this.gameObject.transform.localPosition = position;
    }

    public void MoveUp()
    {
        if (index.y == GRID_SIZE - 1)
        {
            return;
        }
        index.y++;
        // Vector3 position = this.gameObject.transform.localPosition;
        // position.y += 1f;
        // this.gameObject.transform.localPosition = position;
    }

    public void MoveDown()
    {
        if (index.y == 0)
        {
            return;
        }
        index.y--;
        // Vector3 position = this.gameObject.transform.localPosition;
        // position.y -= 1f;
        // this.gameObject.transform.localPosition = position;
    }

    public void MoveBackward()
    {
        if (index.z == 0)
        {
            return;
        }
        index.z--;
        // Vector3 position = this.gameObject.transform.localPosition;
        // position.z -= 1f;
        // this.gameObject.transform.localPosition = position;
    }

    public void MoveForward()
    {
        if (index.z == GRID_SIZE - 1)
        {
            return;
        }
        index.z++;
        // Vector3 position = this.gameObject.transform.localPosition;
        // position.z += 1f;
        // this.gameObject.transform.localPosition = position;
        UpdatePosition();
    }

    private void UpdatePosition() {
      Vector3 position = Vector3.zero;
      position.x = index.x * CELL_SIZE;
      position.y = index.y * CELL_SIZE;
      position.z = index.z * CELL_SIZE;
      this.gameObject.transform.localPosition = position;
    }

    // emit a new block at the cursor's current position, if there isn't a block there already
    public void Emit()
    {
        Vector3 cursorPosition = this.gameObject.transform.localPosition;
        if (isGridSpaceEmpty(cursorPosition))
        {
            GameObject prefab = ModuleController.Instance.IsRecording() ? this.moduleCreationBlockPrefab : this.blockPrefab;
            GameObject obj = Instantiate(prefab, this.gameObject.transform.position, Quaternion.identity, BlocklyGrid.Instance.gridSpace) as GameObject;
        }
    }

    // delete the block at the cursor's current position, if there is one
    public void Delete()
    {
        Collider[] colliders = Physics.OverlapSphere(this.gameObject.transform.localPosition, 0.2f);
        foreach (Collider collider in colliders)
        {
            // only destroy emitted blocks, not the cursor/region
            // TODO: look into layer masks
            if (collider.gameObject.tag == "Block")
            {
                Destroy(collider.gameObject);
            }
        }
    }

    // calls appropriate functions to move/delete/emit based on keypress
    // TODO: eventually remove this as this will be replaced by gesture control
    private void Move()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
          OnRecognizeGesture("CreateModule");
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
          OnRecognizeGesture("Left");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
          OnRecognizeGesture("Right");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
          OnRecognizeGesture("Backward");
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
          OnRecognizeGesture("Forward");
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
          OnRecognizeGesture("Down");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
          OnRecognizeGesture("Up");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
          OnRecognizeGesture("Emit");
        }
        // if (Input.GetKeyDown(KeyCode.T))
        // {
        //   OnRecognizeGesture("Delete");
        // }
    }

    // returns true if there is not currently a block at the given position
    private bool isGridSpaceEmpty(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(this.gameObject.transform.localPosition, 0.2f);
        foreach (Collider collider in colliders)
        {
            // TODO: look into layer masks
            if (collider.gameObject.tag == "Block" && !ModuleController.Instance.IsRecording())
            {
                return false;
            }
        }
        return true;
    }

    public Vector3Int CursorIndex()
    {
        return index;
    }
}

}
