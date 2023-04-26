using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour
{
    public ClockHand hand;
    public PolarityBarTicker ticker;

    //Obviously we should change this to an event system when we set that up
    public EnemyScript enemy;

    void Start(){
    }
    
    public void ChangePolarity(int offset)
    {
        ticker.SetTickerOffset(offset);

        //remove this later
        if(ticker.polarity < 1){
            enemy.Outline(false);
        } else {
            enemy.Outline(true);
        }
        //----------

        hand.RotateClockHand(0.20f * Mathf.Abs(offset));
    }

    public void TurnClock(float amount)
    {
        hand.RotateClockHand(amount);
    }


}
