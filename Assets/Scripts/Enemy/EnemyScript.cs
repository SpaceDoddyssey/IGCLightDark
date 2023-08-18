using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Unity.VisualScripting;
using System;

public class EnemyScript : MonoBehaviour
{

    public int health = 50;
    public int baseDamage = 15;
    public HomeWorld homeWorld = HomeWorld.Light;
    public float moveSpeed = 0.5f;
    public float averageAttackSpeed = 0.1f;
    public float attackBounce;
    public float fadeTime;
    public enum HomeWorld
    {
        Light,
        Dark,
        Null,
        Both
    }

    private float AttackSpeed
    {
        get
        {
            float random = UnityEngine.Random.Range(averageAttackSpeed - (averageAttackSpeed / 2), averageAttackSpeed + (averageAttackSpeed / 2));
            return random;
        }

    }


    public SpriteRenderer spriteRender;
    public SpriteRenderer minimapRender;
    public Sprite defaultSprite, outlineSprite;
    public AStarPathfinding pathfinding;
    public bool inPlayerDimension = true;

    private GameState stateObject;

    // Interp related
    private QuickInterpVec3 movementInterp;
    private QuickInterpVec3 attackInterp;
    private bool retreating;
    private Vector3 prevPosition;
    private State currentState;
    private Animator animator;
    private EffectFade fade;
    

    private enum State
    {
        Attacking,
        Moving,
        Idle
    }

    public AStarNode nodeOfIntent { get; private set; }

    private int itemChance = 5; // 0 <= itemChance <= 100. The percent chance of the enemy dropping an item


    void Start() 
    {



        currentState = State.Idle;  
        stateObject = GameObject.Find("Game World Manager").GetComponent<GameState>();
        GameObject spritechild = gameObject.transform.GetChild(0).gameObject;
        spriteRender = spritechild.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        minimapRender = spritechild.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();
        animator = spritechild.transform.GetChild(0).gameObject.GetComponent<Animator>();
        pathfinding = GetComponent<AStarPathfinding>();
        fade = GetComponent<EffectFade>();


        spriteRender.enabled = false;

        

        // var outline = gameObject.AddComponent<Outline>();

        // outline.OutlineMode = Outline.Mode.OutlineAll;
        // outline.OutlineColor = Color.red;
        // outline.OutlineWidth = 30f;

        animator.speed = UnityEngine.Random.Range(0.93f, 1.05f);

    }

    private void FixedUpdate()
    {
        MovementInterp();
        AttackInterp();
    }

    void Update()
    {


        // Visibility code.
        if (homeWorld == HomeWorld.Light && stateObject.polarity >= 1 ||
        (homeWorld == HomeWorld.Dark && stateObject.polarity <= -1))
        {
            // Entity is active, player is in their "world" or dimension.
            inPlayerDimension = true;
            if (spriteRender.enabled == false)
            {
                spriteRender.enabled = true;
                fade.FadeIn(fadeTime);
            }
            minimapRender.color = new Color(minimapRender.color.r, minimapRender.color.g, minimapRender.color.b, 1f);

        }
        else if (homeWorld == HomeWorld.Light && stateObject.polarity <= 0 ||
                (homeWorld == HomeWorld.Dark  && stateObject.polarity >=  0))
        {
            // Entity is inactive since the player does not inhabit their world.
            inPlayerDimension = false;
            if (spriteRender.color.a == 1f)
            {
                fade.FadeOut(false, fadeTime);
            }
            minimapRender.color = new Color(minimapRender.color.r, minimapRender.color.g, minimapRender.color.b, 0.5f);
        }
    }



    public void TakeDamage(int amount){

        health -= amount;
        Debug.Log("The enemy takes " + amount + " damage!");

        if (this.homeWorld == HomeWorld.Dark)
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Enemies/Dark/en_drk_hurt", gameObject);
        }
        else
            FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Enemies/Light/en_lt_hurt", gameObject);
       

        //stateObject.PrintEnemyDamageText(amount, name);
        transform.GetChild(0).GetChild(0).GetComponent<EffectShake>().DoShake(amount * 2, false);
        if (health <= 0){
            GetComponent<BoxCollider>().enabled = false;
            Debug.Log("The enemy dies!");
            FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Enemies/en_death", gameObject);
            fade.FadeOut(true, 0.3f);
            int dieroll = UnityEngine.Random.Range(1, 101);
            if(dieroll <= itemChance){
                stateObject.AcquireRandItem();
            }
        }
    }

    // Activated whenever the clock hits twelve, called from a UnityEvent.
    public void OnClockTwelve()
    {
        if (inPlayerDimension && currentState == State.Idle)
        {
            pathfinding.FindPath(transform.position, GameObject.Find("Player").transform.position);

            if (pathfinding.path.Count > 1)
            {
                // Check to see if enemy in the same dimension is blocking you.

                {
                    RaycastHit[] hits = Physics.RaycastAll(transform.position, pathfinding.path[0].worldPosition - transform.position, 2f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);

                    foreach (RaycastHit hit in hits)
                    {
                        EnemyScript e = hit.collider.GetComponent<EnemyScript>();
                        if (e != null)
                        {
                            // If the enemy you're looking at is in your dimension, then you can't pass thru them
                            if (e.homeWorld == gameObject.GetComponent<EnemyScript>().homeWorld)
                            {
                                return;
                            }
                        }

                    }
                }

                // Check all other enemy scripts to see if the node is claimed.
                foreach (EnemyScript e in transform.parent.GetComponentsInChildren<EnemyScript>())
                {
                    if (e.nodeOfIntent == pathfinding.path[0])
                    {
                        return;
                    }
                }

                // Otherwise if the node is not yet claimed, then move!
                prevPosition = transform.position;
                nodeOfIntent = pathfinding.path[0];
                movementInterp = new QuickInterpVec3(transform.position, pathfinding.path[0].worldPosition, moveSpeed, true);
                currentState = State.Moving;
                animator.Play("enemy_move");
                if (this.homeWorld == HomeWorld.Dark)
                {
                    FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Enemies/Dark/en_drk_move", gameObject);
                }
                else
                    FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Enemies/Light/en_lt_move", gameObject);

            }
            // This is kind of a buggy check to see if the enemy is right next to the player.
            else if (pathfinding.path.Count == 1)
            {
                Attack(pathfinding.path[0].worldPosition);
            }
        }
    }

    private void MovementInterp()
    {
        // If an interp has been made, do stuff with it.
        if (movementInterp != null)
        {


            RaycastHit hit = new RaycastHit();

            // More adjustment for origin fuckery
            Vector3 originAdjusted = new Vector3(movementInterp.startValue.x, movementInterp.startValue.y, movementInterp.startValue.z);
            originAdjusted.y += 1f;

            Vector3 dir = movementInterp.endValue - movementInterp.startValue;

            Debug.DrawRay(originAdjusted, dir.normalized);

            if (retreating == false && Physics.Raycast(originAdjusted, dir.normalized, out hit, 2f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal))
            {
                if (hit.collider.name == "Player")
                {
                    movementInterp = new QuickInterpVec3(transform.position, prevPosition, moveSpeed / 2, false);
                    retreating = true;
                }

            }

            // Slowly move the transform to the new position.
            transform.position = movementInterp.status;
            // If it's done, then reset everything.
            if (movementInterp.isDone)
            {
                movementInterp = null;
                nodeOfIntent = null;
                currentState = State.Idle;
                retreating = false;
                return;
            }

            // Otherwise, keep interping.
            movementInterp.InterpUpdate();

        }
    }
    
    private void Attack(Vector3 targetPos)
    {
        prevPosition = spriteRender.transform.position;
        currentState = State.Attacking;
        animator.Play("enemy_bite");


        //This is a fix for a glitch involving origins.
        targetPos.y += 1f;

        // Add a bit of extra bump so the imp goes out more.
        Vector3 difference = targetPos - spriteRender.transform.position;

        // Start an interp here.
        attackInterp = new QuickInterpVec3(spriteRender.transform.position, targetPos - (difference.normalized) + (difference.normalized * attackBounce), AttackSpeed, true);
    }

    private void AttackInterp()
    {
        if (attackInterp != null)
        {
            spriteRender.transform.position = attackInterp.status;
            if (retreating)
            {
                if (attackInterp.isDone)
                {
                    // If part two of the interp is done (the retreating part), reset everything.
                    attackInterp = null;
                    currentState = State.Idle;
                    retreating = false;
                    spriteRender.transform.position = prevPosition;
                    return;
                }
            }
            else
            {
                // Otherwise, we must be on the ATTACKING part of the attack interp, moving forward basically.
                // However, if that's done, then DAMAGE THE PLAYER and start retreating.
                // Don't do the full interp because we don't want the imp going all the way into the square.
                if (attackInterp.duration >= 0.80)
                {
                    RaycastHit hit = new RaycastHit();
                    Vector3 dir = attackInterp.endValue - attackInterp.startValue;
                    Debug.DrawRay(attackInterp.startValue, dir);
                    if (Physics.Raycast(attackInterp.startValue, dir.normalized, out hit, 2f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal))
                    {
                        if (hit.collider.name == "Player")
                        {
                            // Attack the player. Damage direction indicates whether the damage is positive or negative
                            int damageDirection = (homeWorld == HomeWorld.Light ? 1 : -1);
                            stateObject.DamagePlayer((int)(baseDamage * damageDirection * (stateObject.GetDamageModifier()) * UnityEngine.Random.Range(0.90f, 1.05f)), name);

                            if (this.homeWorld == HomeWorld.Dark)
                            {
                                FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Enemies/Dark/en_drk_atk", gameObject);
                            }
                            else
                                FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Enemies/Light/en_lt_atk", gameObject);

                            if (Mathf.Abs(stateObject.polarity) == 3)
                                FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Attack/atk_med", gameObject);

                            if (Mathf.Abs(stateObject.polarity) == 4)
                                FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Attack/atk_heavy", gameObject);


                        }

                    }

                    // Reverse it with a new attack interp here, overriding the other one.
                    retreating = true;
                    attackInterp = new QuickInterpVec3(spriteRender.transform.position, prevPosition, AttackSpeed, true);

                }
            }

            // Otherwise, keep interping.
            attackInterp.InterpUpdate();
        }
    }

}