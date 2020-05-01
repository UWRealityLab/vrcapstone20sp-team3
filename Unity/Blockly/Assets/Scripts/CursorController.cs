using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public float gridSize;
    public bool isSelected;  // whether this block is currently selected

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Select();
        Move();
    }

    // if mouse click on a block, set that block to be the currently selected block
    private void Select()
    {
        // TODD: update the if condition + Raycast logic to use gestures
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse is down");

            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {
                Debug.Log("Hit " + hitInfo.transform.gameObject.name);
                if (hitInfo.collider.gameObject.tag == "Selectable")
                {
                    this.isSelected = this.gameObject == hitInfo.collider.gameObject;
                    Debug.Log("It's working!");
                }
                else
                {
                    Debug.Log("nopz");
                }
            }
            else
            {
                Debug.Log("No hit");
            }
        }
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

    public void MoveForward()
    {
        Vector3 position = this.gameObject.transform.position;
        position.y += gridSize;
        this.gameObject.transform.position = position;
    }

    public void MoveBackward()
    {
        Vector3 position = this.gameObject.transform.position;
        position.y -= gridSize;
        this.gameObject.transform.position = position;
    }

    public void MoveUp()
    {
        Vector3 position = this.gameObject.transform.position;
        position.z += gridSize;
        this.gameObject.transform.position = position;
    }

    public void MoveDown()
    {
        Vector3 position = this.gameObject.transform.position;
        position.z -= gridSize;
        this.gameObject.transform.position = position;
    }

    public void Remove()
    {
        this.isSelected = false;
        Destroy(this.gameObject);
    }

    // if key press for move/delete, perform action on this block
    // does nothing if block is not currently selected
    private void Move()
    {
        if (!this.isSelected)
        {
            return;
        }

        // TODO: update the if conditions to be based on gestures rather than keypresses
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

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Remove();
        }
    }
}

