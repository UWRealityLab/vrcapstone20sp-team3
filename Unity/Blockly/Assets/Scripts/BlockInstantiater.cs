using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInstantiater : MonoBehaviour
{

    private Ray ray;
    private RaycastHit hit;
    public GameObject blockPrefab;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // TODO change if condition + Raycast logic to gestures
        if (Input.GetKeyDown(KeyCode.R))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                // TODO: change hit.point.x, y, z to the location of the gesture
                Vector3 location = snapToGrid(new Vector3(hit.point.x, hit.point.y, hit.point.z));
                if (isGridSpaceEmpty(location))
                {
                    GameObject obj = Instantiate(blockPrefab, location, Quaternion.identity) as GameObject;
                }
            }
        }
    }

    // takes a position and returns a new position "snapped" to a grid
    // (converts x, y, z of position into whole numbers)
    private Vector3 snapToGrid(Vector3 position)
    {
        return new Vector3((float) Math.Ceiling(position.x),
                           (float) Math.Ceiling(position.y),
                           (float) Math.Ceiling(position.z));
    }

    // returns true if there is not currently a block at the given position
    private bool isGridSpaceEmpty(Vector3 position)
    {
        return Physics.CheckSphere(position, 0.2f);
    }
}