using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    SpriteRenderer spriteRend;
    public Sprite openSprite, closedSprite;

    void Start(){
        spriteRend = gameObject.GetComponent<SpriteRenderer>();
    }

    public void Open(){
        spriteRend.sprite = openSprite;
        Debug.Log("HELLO!");
        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    public void Close(){
        spriteRend.sprite = closedSprite;
        gameObject.layer = LayerMask.NameToLayer("Unwalkable");
    }
}
