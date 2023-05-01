using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    int hp = 3;
    public SpriteRenderer spriteRender;
    public Sprite defaultSprite, outlineSprite;
    public AStarPathfinding pathfinding;

    void Start() {

        GameObject spritechild = gameObject.transform.GetChild(0).gameObject;
        spriteRender = spritechild.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();

        pathfinding = GetComponent<AStarPathfinding>();

        //Establish player target
        pathfinding.seeker = transform;
        pathfinding.target = GameObject.Find("Player").transform;

        // var outline = gameObject.AddComponent<Outline>();

        // outline.OutlineMode = Outline.Mode.OutlineAll;
        // outline.OutlineColor = Color.red;
        // outline.OutlineWidth = 30f;


    }

    public void Outline(bool outl){
        if(outl) {
            spriteRender.sprite = outlineSprite;
        } else {
            spriteRender.sprite = defaultSprite;
        }
    }

    public void TakeDamage(int amount){
        hp -= amount;
        Debug.Log("The imp takes " + amount + " damage!");
        if (hp < 0){
            Debug.Log("The imp dies!");
            Destroy(gameObject);
        }
    }

    // Activated whenever the clock hits twelve, called from a UnityEvent.
    public void OnClockTwelve()
    {
        pathfinding.FindPath();
        if (pathfinding.path.Count > 1)
        {
            transform.position = pathfinding.path[0].worldPosition;
        }
        else if (pathfinding.path.Count == 1)
        {
            // This is kind of a buggy check to see if the enemy is right next to the player.

        }
    }

}