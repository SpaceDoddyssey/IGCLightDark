using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameState : MonoBehaviour
{
    public ClockHand hand;
    public PolarityBarTicker ticker;
    public int playerHealth = 0;

    public int playerMaxAbsoluteHealth = 100;
    public int polarity;

    public Vector3 clockRotation = new Vector3(0f, 0f, 0f);


    void Start(){
    }

    public float GetDamageModifier()
    {
        switch(Mathf.Abs(polarity))
        {
            case 0:
                return 0;
            case 1:
                return .5f;
            case 2:
                return .75f;
            case 3:
                return 1.25f;
            case 4:
                return 2f;
            default:
                return 1f;
        }


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

    public void DamagePlayer(int damage)
    {
        playerHealth += damage;
        print("Player takes " + damage + " damage!");
        if (playerHealth > playerMaxAbsoluteHealth || playerHealth < playerMaxAbsoluteHealth * -1)
        {
            print("Threshhold broken! Game over!");
            Application.Quit();
        }

    }

}
