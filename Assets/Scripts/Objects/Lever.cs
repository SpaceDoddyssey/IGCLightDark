using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour
{
    public List<DoorScript> connectedDoors;
    SpriteRenderer spriteRend;
    bool beenPulled = false;
    public Sprite unpulledSprite, pulledSprite;

    void Start(){
        spriteRend = gameObject.GetComponentInChildren<SpriteRenderer>();
    }

    public void Pull(){
        if (beenPulled) return;
        if(!beenPulled){
            spriteRend.sprite = pulledSprite;
            foreach (DoorScript d  in connectedDoors) 
            {
               d.Open();
                FMODUnity.RuntimeManager.PlayOneShotAttached("event:/World/Lever/lever", gameObject);
            }
            
        }

        beenPulled=true;
    }   
}
