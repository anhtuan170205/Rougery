using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WallObject : CellObject
{
    public Tile obstacleTile;
    public Tile damagedTile;
    public int maxHitPoints = 3;
    private int hitPoints;
    private Tile originalTile;
    public override void PlayerEntered()
    {
        Debug.Log("Player hit a wall");
    }
    public override void Init(Vector2Int cell)
    {
        base.Init(cell);
        hitPoints = maxHitPoints;
        originalTile = GameManager.instance.boardManager.GetCellTile(cell);
        GameManager.instance.boardManager.SetCellTile(cell, obstacleTile);
    }
    public override bool PlayerWantToEnter()
    {
        hitPoints--;
        GameManager.instance.player.TriggerDigAnimation();
        AudioPlayer.instance.PlayDigClip(transform.position);
        if (hitPoints == 1)
        {
            GameManager.instance.boardManager.SetCellTile(cell, damagedTile);
        }
        if (hitPoints > 0)
        {
            return false;
        }
        GameManager.instance.boardManager.SetCellTile(cell, originalTile);
        Destroy(gameObject);
        return true;
    }
}
