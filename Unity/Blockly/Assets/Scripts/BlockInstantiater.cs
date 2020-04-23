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
                GameObject obj = Instantiate(blockPrefab, new Vector3(hit.point.x, hit.point.y, hit.point.z), Quaternion.identity) as GameObject;
            }
        }
    }
}