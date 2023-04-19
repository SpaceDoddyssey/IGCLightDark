using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour
{
    public ClockHand hand;
    public PolarityBarTicker ticker;



    void Start(){
    }
    
    public void ChangePolarity(int offset)
    {
        ticker.SetTickerOffset(offset);
        hand.RotateClockHand(0.20f * Mathf.Abs(offset));
    }

    public void TurnClock(float amount)
    {
        hand.RotateClockHand(amount);
    }


}
