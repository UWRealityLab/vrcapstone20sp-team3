using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordController : MonoBehaviour
{
    //private List<>
    // Start is called before the first frame update
    //void Start()
    //{
        
    //}
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
                if (Object.ReferenceEquals(hitInfo.collider.gameObject, this.gameObject))
                {
                    
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

    // Update is called once per frame
    void Update()
    {
        CursorController[] blocks = FindObjectsOfType<CursorController>();
        GameObject selectedBlock;
        foreach (CursorController b in blocks)
        {
            if (b.isSelected)
            {
                selectedBlock = b.gameObject;
                Debug.Log(selectedBlock);
            }
        }
    }
}
