using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public enum HomeWorld
    {
        Light,
        Dark
    }
    public int health = 50;
    public int baseDamage = 15;
    public HomeWorld homeWorld = HomeWorld.Light;

    public SpriteRenderer spriteRender;
    public Sprite defaultSprite, outlineSprite;
    public AStarPathfinding pathfinding;
    public bool inPlayerDimension = true;

    private GameState stateObject;

    void Start() {

        stateObject = GameObject.Find("Game World Manager").GetComponent<GameState>();
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


    void Update()
    {
            if (homeWorld == HomeWorld.Light && stateObject.polarity >= 1 ||
           (homeWorld == HomeWorld.Dark && stateObject.polarity <= -1))
        {
            // Entity is active, player is in their "world" or dimension.
            inPlayerDimension = true;
            spriteRender.enabled = true;

        }
        else if (homeWorld == HomeWorld.Light && stateObject.polarity <= 0 ||
                (homeWorld == HomeWorld.Dark  && stateObject.polarity >=  0))
        {
            // Entity is inactive since the player does not inhabit their world.
            inPlayerDimension = false;
            spriteRender.enabled = false;
        }
    }

    public void Outline(bool outl){
        if(outl) {
            spriteRender.sprite = outlineSprite;
        } else {
            spriteRender.sprite = defaultSprite;
        }
    }

    public void TakeDamage(int amount){
        health -= amount;
        Debug.Log("The imp takes " + amount + " damage!");
        if (health < 0){
            Debug.Log("The imp dies!");
            Destroy(gameObject);
        }
    }

    // Activated whenever the clock hits twelve, called from a UnityEvent.
    public void OnClockTwelve()
    {
        if (inPlayerDimension)
        {
            pathfinding.FindPath();
            if (pathfinding.path.Count > 1)
            {
                transform.position = pathfinding.path[0].worldPosition;
            }
            // This is kind of a buggy check to see if the enemy is right next to the player.
            else if (pathfinding.path.Count == 1)
            {
                // Attack the player.
                int damageDirection = (homeWorld == HomeWorld.Light ? -1 : 1);
                stateObject.DamagePlayer((int)(baseDamage * damageDirection * stateObject.GetDamageModifier()));
            }
        }

    }

}