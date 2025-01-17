using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodObject : CellObject
{
    public int foodValue = 10;
    public override void PlayerEntered()
    {
        Destroy(gameObject);
        Debug.Log("Player ate food");
        GameManager.instance.ChangeFood(foodValue);
        AudioPlayer.instance.PlayEatClip(transform.position);
    }
}
