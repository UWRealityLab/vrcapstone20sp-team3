using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour
{
    private Vector3 cursor;
    private Vector3 maximum;  // max x, y, z offset
    private Vector3 minimum;  // min x, y, z offset
    private List<string> statements;

    private const float GRID_SIZE = 1f;

    public Module()
    {
        this.cursor = Vector3.zero;
        this.maximum = Vector3.zero;
        this.minimum = Vector3.zero;
        this.statements = new List<string>();
    }

    // appends one statement to the module
    // updates the max or min x/y/z value accordingly
    public void AddStatement(string action)
    {
        this.statements.Add(action);

        switch (action)
        {
            case "Emit":
                break;
            // case "Delete":
            //     break;
            case "Right":
                cursor.x += GRID_SIZE;
                break;
            case "Left":
                cursor.x -= GRID_SIZE;
                break;
            case "Up":
                cursor.y += GRID_SIZE;
                break;
            case "Down":
                cursor.y -= GRID_SIZE;
                break;
            case "Forward":
                cursor.z += GRID_SIZE;
                break;
            case "Backward":
                cursor.z -= GRID_SIZE;
                break;
            default:
                break;
        }
        this.maximum = Vector3.Max(cursor, maximum);
        this.minimum = Vector3.Min(cursor, minimum);
    }

    public List<string> Statements()
    {
        return this.statements;
    }

    // returns the "maximum" corner position of the module relative to the
    // given position as the start position of the cursor for the module
    public Vector3 MaxPositionFromStart(Vector3 startPos)
    {
        return startPos + this.maximum;
    }

    // returns the "minimum" corner position of the module relative to the
    // given position as the start position of the cursor for the module
    public Vector3 MinPositionFromStart(Vector3 startPos)
    {
        return startPos + this.minimum;
    }

    // return the start position for a cursor to recreate this module
    // so that the "minimum" corner of the module is at the given minCorner
    public Vector3 StartPositionFromMinCorner(Vector3 minCorner)
    {
        return minCorner - this.minimum;
    }

    public Vector3 TotalSize()
    {
        return this.maximum - this.minimum;
    }

    public Vector3 Maximum()
    {
        return this.maximum;
    }

    public Vector3 Minimum()
    {
        return this.minimum;
    }

    public int Size()
    {
        return this.statements.Count;
    }
}
