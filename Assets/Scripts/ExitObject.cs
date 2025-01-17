using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ExitObject : CellObject
{
    public Tile exitTile;
    public override void Init(Vector2Int cell)
    {
        base.Init(cell);
        GameManager.instance.boardManager.SetCellTile(cell, exitTile);
    }
    public override void PlayerEntered()
    {
        Debug.Log("Player entered the exit");
        GameManager.instance.NewLevel();
    }
}
