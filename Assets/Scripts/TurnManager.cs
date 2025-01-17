using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager
{
    public event System.Action OnTick;
    private int turnCount;
    public TurnManager()
    {
        turnCount = 1;
    }
    public void Tick()
    {
        turnCount++;
        if (OnTick != null)
        {
            OnTick.Invoke();
        }
    }
}
