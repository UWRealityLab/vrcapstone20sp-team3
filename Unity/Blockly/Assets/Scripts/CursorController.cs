using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blockly {

public class CursorController : MonoBehaviour
{
    public static CursorController Instance = null;

    [NotNull]
    public GameObject blockPrefab;  // prefab for blocks, used when emitted
    [NotNull]
    public GameObject moduleCreationBlockPrefab;  // prefab for the temporary blocks that show up during module creation

    public GameObject recordButton;

    private HashSet<Vector3> blockPositions;  // the positions for all blocks existing on grid right now

    public AudioClip emitSound;
    public AudioClip moveSound;

    private Vector3Int cursorIdx;

    private AudioSource source;

    void Awake()
    {
        Debug.Assert(Instance == null, "singleton class instantiated multiple times");
        Instance = this;
        source = GetComponent<AudioSource>();
        blockPositions = new HashSet<Vector3>();
        cursorIdx = Vector3Int.zero;
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

        Vector3Int oldCursorIdx = cursorIdx;
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
            if (gestureName != "Emit" && cursorIdx == oldCursorIdx)
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
        if (cursorIdx.x == BlocklyGrid.GRID_SIZE - 1)
        {
            return;
        }
        cursorIdx.x++;
        // Vector3 position = this.gameObject.transform.localPosition;
        // position.x += 1f;
        // this.gameObject.transform.localPosition = position;
    }

    public void MoveLeft()
    {
        if (cursorIdx.x == 0)
        {
            return;
        }
        cursorIdx.x--;
        // Vector3 position = this.gameObject.transform.localPosition;
        // position.x -= 1f;
        // this.gameObject.transform.localPosition = position;
    }

    public void MoveUp()
    {
        if (cursorIdx.y == BlocklyGrid.GRID_SIZE - 1)
        {
            return;
        }
        cursorIdx.y++;
        // Vector3 position = this.gameObject.transform.localPosition;
        // position.y += 1f;
        // this.gameObject.transform.localPosition = position;
    }

    public void MoveDown()
    {
        if (cursorIdx.y == 0)
        {
            return;
        }
        cursorIdx.y--;
        // Vector3 position = this.gameObject.transform.localPosition;
        // position.y -= 1f;
        // this.gameObject.transform.localPosition = position;
    }

    public void MoveBackward()
    {
        if (cursorIdx.z == 0)
        {
            return;
        }
        cursorIdx.z--;
        // Vector3 position = this.gameObject.transform.localPosition;
        // position.z -= 1f;
        // this.gameObject.transform.localPosition = position;
    }

    public void MoveForward()
    {
        if (cursorIdx.z == BlocklyGrid.GRID_SIZE - 1)
        {
            return;
        }
        cursorIdx.z++;
        // Vector3 position = this.gameObject.transform.localPosition;
        // position.z += 1f;
        // this.gameObject.transform.localPosition = position;
        UpdatePosition();
    }

    private void UpdatePosition() {
      Vector3 position = Vector3.zero;
      position.x = cursorIdx.x;
      position.y = cursorIdx.y;
      position.z = cursorIdx.z;
      this.gameObject.transform.localPosition = position;
    }

    public Vector3 PositionFromCursorIdx(Vector3Int idx) {
        return transform.parent.TransformPoint(idx.x, idx.y, idx.z);
    }

    // emit a new block at the cursor's current position, if there isn't a block there already
    // if there is a block there, deletes it
    public void Emit()
    {
        bool blockExisted = false;
        Collider[] colliders = BlocklyGrid.Instance.BlocksAtIndex(cursorIdx);
        foreach (Collider collider in colliders)
        {
            if (ModuleController.Instance.IsRecording() ? collider.gameObject.tag == "Module Creation Block" : collider.gameObject.tag == "Block")
            {
                if (blockPositions.Contains(transform.position))
                {
                    blockExisted = true;
                    Destroy(collider.gameObject);
                }
                break;
            }
        }

        if (!blockExisted)
        {
            GameObject prefab = ModuleController.Instance.IsRecording() ? this.moduleCreationBlockPrefab : this.blockPrefab;
            GameObject obj = Instantiate(prefab, transform.position, Quaternion.identity, BlocklyGrid.Instance.gridSpace) as GameObject;
            blockPositions.Add(transform.position);
        }
    }

    // delete the block at the cursor's current position, if there is one
    public void Delete()
    {
        Collider[] colliders = BlocklyGrid.Instance.BlocksAtPosition(this.gameObject.transform.position);
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
        Collider[] colliders = BlocklyGrid.Instance.BlocksAtPosition(this.gameObject.transform.position);
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
        return cursorIdx;
    }

    public void SetCursorIndex(Vector3Int idx) {
        cursorIdx = idx;
        UpdatePosition();
    }

    public Vector3 CursorPosition()
    {
        return transform.position;
    }
}

}
