using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : BaseItem
{
    public int healAmount = 50;

    public override bool Use(){
        GameState stateScript = GameObject.Find("Game World Manager").GetComponent<GameState>();

        int damage = System.Math.Abs(stateScript.playerHealth);
        if(damage == 0){ 
            return false; 
        } 
        else if (damage <= healAmount){
            stateScript.playerHealth = 0;
        } 
        else if (stateScript.playerHealth < 0){
            stateScript.playerHealth += healAmount;
        } 
        else {
            stateScript.playerHealth -= healAmount;
        }
        return true;
    }
}
