using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    SpriteRenderer spriteRend;
    Animator anim;
    SpriteRenderer minimap;
    public Sprite openSprite, closedSprite;
    public bool IsOpen
    {
        get; private set;
    } = false;

    void Start(){
        spriteRend = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        anim = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
        minimap = transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>();
        IsOpen = false;
    }

    public void Open(){

        if (IsOpen) return;

        anim.Play("fade");
        gameObject.layer = LayerMask.NameToLayer("Default");
        minimap.color = new Color(0f, 0, 0f, 0f);
        IsOpen = true;
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/World/Door/dr_open", gameObject);
    }

    public void Close(){
        spriteRend.sprite = closedSprite;
        gameObject.layer = LayerMask.NameToLayer("Unwalkable");
        IsOpen = false;
    }

   
}
