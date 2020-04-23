using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    public float gridSize;
    private GameObject selectedBlock;  // currently selected block

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Select();
        Move(this.selectedBlock);
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
                    this.selectedBlock = hitInfo.collider.gameObject;
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

    // if key press for move/delete, perform action on the currently selected block
    // does nothing if no block is currently selected
    private void Move(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return;
        }

        // TODO: update the if conditions to be based on gestures rather than keypresses
        if (Input.GetKeyDown(KeyCode.A))
        {
            Vector3 position = gameObject.transform.position;
            position.x -= gridSize;
            gameObject.transform.position = position;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Vector3 position = gameObject.transform.position;
            position.x += gridSize;
            gameObject.transform.position = position;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Vector3 position = gameObject.transform.position;
            position.y -= gridSize;
            gameObject.transform.position = position;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Vector3 position = gameObject.transform.position;
            position.y += gridSize;
            gameObject.transform.position = position;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Vector3 position = gameObject.transform.position;
            position.z -= gridSize;
            gameObject.transform.position = position;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector3 position = gameObject.transform.position;
            position.z += gridSize;
            gameObject.transform.position = position;
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Destroy(gameObject);
        }
    }
}
