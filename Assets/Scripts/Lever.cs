using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour
{
    public DoorScript connectedDoor;
    SpriteRenderer spriteRend;
    bool beenPulled = false;
    public Sprite unpulledSprite, pulledSprite;

    void Start(){
        spriteRend = gameObject.GetComponentInChildren<SpriteRenderer>();
    }

    public void Pull(){
        if(!beenPulled){
            spriteRend.sprite = pulledSprite;
            connectedDoor.Open();
        }
    }   
}
