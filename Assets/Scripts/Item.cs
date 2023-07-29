using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Potion, 
    FireballScroll,
    None //If you add more always put None last
}

public static class Item
{
    // static Sprite[] itemSprites;

    // static Item(){
    //     var potionSprite = Resources.Load("ph_potion") as Sprite;
    //     var fireballSprite = Resources.Load("ph_fireball_scroll") as Sprite;
    //     itemSprites = new Sprite[] { potionSprite, fireballSprite };
    // }

    //public static (ItemType, Sprite) RandomItemData(){
    public static ItemType RandomItemData(){
        int type = Random.Range(0, (int)ItemType.None); //Update this number if you add more item types
        //return ((ItemType)type, itemSprites[type]); 
        return (ItemType)type;
    }

    public static bool Use(ItemType t){
        switch(t){
            case ItemType.Potion: 
                return UsePotion();
            case ItemType.FireballScroll:
                return UseFireball();
            default: 
                Debug.Log("Default!"); 
                return false;
        }
    }

    public static bool UseFireball(){
        GameObject player = GameObject.Find("Player");
        GameObject fballPrefab = Resources.Load("Fireball", typeof(GameObject)) as GameObject;
        GameObject fball = Object.Instantiate(fballPrefab, player.transform.position, Quaternion.identity);
        FireballScript fScript= fball.GetComponent<FireballScript>();
        Debug.Log("Item script says: " + player.transform.forward);
        fScript.dir = player.transform.forward;
        return true;        
    }

    public static bool UsePotion(){
        int healAmount = 50;

        GameState stateScript = GameObject.Find("Game World Manager").GetComponent<GameState>();

        //The potion brings you up to "healAmount" closer to the center of the health bar
        int damage = System.Math.Abs(stateScript.playerHealth);
        if(damage == 0){
            //Does not use up the item if you're already at 0 damage
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
