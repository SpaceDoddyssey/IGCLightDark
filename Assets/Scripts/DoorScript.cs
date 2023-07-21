using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    SpriteRenderer spriteRend;
    Animator anim;
    SpriteRenderer minimap;
    public Sprite openSprite, closedSprite;

    void Start(){
        spriteRend = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        anim = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
        minimap = transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>();
    }

    public void Open(){
        
        Debug.Log("HELLO!");
        anim.Play("fade");
        gameObject.layer = LayerMask.NameToLayer("Default");
        minimap.color = new Color(0f, 0, 0f, 0f);
    }

    public void Close(){
        spriteRend.sprite = closedSprite;
        gameObject.layer = LayerMask.NameToLayer("Unwalkable");
    }
}
