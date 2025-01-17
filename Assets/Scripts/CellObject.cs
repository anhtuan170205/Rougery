using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellObject : MonoBehaviour
{
    protected Vector2Int cell;
    public virtual void Init(Vector2Int cell)
    {
        this.cell = cell;
    }
    public virtual void PlayerEntered()
    {
        
    }
    public virtual bool PlayerWantToEnter()
    {
        return true;
    }
}
