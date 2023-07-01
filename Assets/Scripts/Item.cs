using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    None,
    Potion, 
    FireballScroll
}

public class Item : MonoBehaviour
{
    public GameObject itemPrefab;
    public ItemType itemType;

    public static bool Use(ItemType t){
        switch(t){
            case ItemType.Potion: 
                return UsePotion();
            default: 
                print("Default!"); 
                return false;
        }
    }

    public static bool UsePotion(){
        int healAmount = 50;

        GameState stateScript = GameObject.Find("Game World Manager").GetComponent<GameState>();

        

        //The potion brings you up to "healAmount" closer to the center of the health bar
        int damage = System.Math.Abs(stateScript.playerHealth);
        if(damage == 0){
            Debug.Log("No damage to heal!");
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
        Debug.Log("Used a Potion!");
        return true;
    }
}
