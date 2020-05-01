using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public float gridSize = 1.0f;
    public GameObject blockPrefab;  // prefab for blocks, used when emitted

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    
    public void MoveRight()
    {
        Vector3 position = this.gameObject.transform.position;
        position.x += gridSize;
        this.gameObject.transform.position = position;
    }

    public void MoveLeft()
    {
        Vector3 position = this.gameObject.transform.position;
        Debug.Log("old pos = " + this.gameObject.transform.position);
        position.x -= gridSize;
        this.gameObject.transform.position = position;
        Debug.Log("new pos = " + this.gameObject.transform.position);
    }

    public void MoveUp()
    {
        Vector3 position = this.gameObject.transform.position;
        position.y += gridSize;
        this.gameObject.transform.position = position;
    }

    public void MoveDown()
    {
        Vector3 position = this.gameObject.transform.position;
        position.y -= gridSize;
        this.gameObject.transform.position = position;
    }

    public void MoveBackward()
    {
        Vector3 position = this.gameObject.transform.position;
        position.z += gridSize;
        this.gameObject.transform.position = position;
    }

    public void MoveForward()
    {
        Vector3 position = this.gameObject.transform.position;
        position.z -= gridSize;
        this.gameObject.transform.position = position;
    }

    // emit a new block at the cursor's current position, if there isn't a block there already
    public void Emit()
    {
        Vector3 cursorPosition = this.gameObject.transform.position;
        if (isGridSpaceEmpty(cursorPosition))
        {
            GameObject obj = Instantiate(blockPrefab, this.gameObject.transform.position, Quaternion.identity) as GameObject;
        }
    }

    // delete the block at the cursor's current position, if there is one
    public void Delete()
    {
        Collider[] colliders = Physics.OverlapSphere(this.gameObject.transform.position, 0.2f);
        foreach (Collider collider in colliders)
        {
            // only destroy emitted blocks, not the cursor/region
            // TODO: look into layer masks
            if (collider.gameObject.tag == "Selectable")
            {
                Destroy(collider.gameObject);
            }
        }
    }

    // calls appropriate functions to move/delete/emit based on keypress
    // TODO: eventually remove this as this will be replaced by gesture control
    private void Move()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveLeft();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            MoveRight();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            MoveBackward();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveForward();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            MoveDown();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            MoveUp();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Emit();
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Delete();
        }
    }

    // returns true if there is not currently a block at the given position
    private bool isGridSpaceEmpty(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(this.gameObject.transform.position, 0.2f);
        foreach (Collider collider in colliders)
        {
            // TODO: look into layer masks
            if (collider.gameObject.tag == "Selectable")
            {
                return false;
            }
        }
        return true;
    }
}

