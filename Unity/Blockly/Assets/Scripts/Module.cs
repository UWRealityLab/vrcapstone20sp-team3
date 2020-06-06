using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blockly {

public class Module : MonoBehaviour
{
    private Vector3Int cursor;
    private Vector3Int maximum;  // max x, y, z offset
    private Vector3Int minimum;  // min x, y, z offset
    private List<string> statements;

    public Module()
    {
        this.cursor = Vector3Int.zero;
        this.maximum = Vector3Int.zero;
        this.minimum = Vector3Int.zero;
        this.statements = new List<string>();
    }

    // appends one statement to the module
    // updates the max or min x/y/z value accordingly
    public void AddStatement(string action)
    {
        if (action == "Emit" && this.statements.Count > 0 && this.statements[this.statements.Count - 1] == "Emit")
        {
            this.statements.RemoveAt(this.statements.Count - 1);
            return;
        }

        this.statements.Add(action);

        switch (action)
        {
            case "Emit":
                this.maximum = Vector3Int.Max(cursor, maximum);
                this.minimum = Vector3Int.Min(cursor, minimum);
                break;
            case "Right":
                cursor.x++;
                break;
            case "Left":
                cursor.x--;
                break;
            case "Up":
                cursor.y++;
                break;
            case "Down":
                cursor.y--;
                break;
            case "Forward":
                cursor.z++;
                break;
            case "Backward":
                cursor.z--;
                break;
            default:
                break;
        }
    }

    // call this method to "complete" the module
    // updates max/min to account for end cursor position of the module
    public void Complete()
    {
        this.maximum = Vector3Int.Max(cursor, maximum);
        this.minimum = Vector3Int.Min(cursor, minimum);
    }

    public List<string> Statements()
    {
        return this.statements;
    }

    // returns the "maximum" corner position of the module relative to the
    // given position as the start position of the cursor for the module
    public Vector3Int MaxIndexFromStart(Vector3Int startIdx)
    {
        return startIdx + this.maximum;
    }

    // returns the "minimum" corner position of the module relative to the
    // given position as the start position of the cursor for the module
    public Vector3Int MinIndexFromStart(Vector3Int startIdx)
    {
        return startIdx + this.minimum;
    }

    // return the start position for a cursor to recreate this module
    // so that the "minimum" corner of the module is at the given minCorner
    public Vector3Int StartIndexFromMinCorner(Vector3Int minCorner)
    {
        return minCorner - this.minimum;
    }

    // returns a vector representing the total size of the module in the
    // x, y, z dimensions
    public Vector3Int TotalSize()
    {
        return this.maximum - this.minimum;
    }
}
}
