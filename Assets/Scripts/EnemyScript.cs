using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyScript : MonoBehaviour
{

    public int health = 50;
    public int baseDamage = 15;
    public HomeWorld homeWorld = HomeWorld.Light;
    public float moveSpeed = 0.5f;
    public float attackSpeed = 0.1f;
    public float attackBounce;
    public enum HomeWorld
    {
        Light,
        Dark,
        Null,
        Both
    }


    public SpriteRenderer spriteRender;
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
    

    private enum State
    {
        Attacking,
        Moving,
        Idle
    }

    public AStarNode nodeOfIntent { get; private set; }



    void Start() 
    {
        currentState = State.Idle;
        stateObject = GameObject.Find("Game World Manager").GetComponent<GameState>();
        GameObject spritechild = gameObject.transform.GetChild(0).gameObject;
        spriteRender = spritechild.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();

        pathfinding = GetComponent<AStarPathfinding>();


        // var outline = gameObject.AddComponent<Outline>();

        // outline.OutlineMode = Outline.Mode.OutlineAll;
        // outline.OutlineColor = Color.red;
        // outline.OutlineWidth = 30f;


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
        stateObject.PrintEnemyDamageText(amount, name);
        if (health <= 0){
            Debug.Log("The imp dies!");
            Destroy(gameObject);
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
                // There's a bug around here... Where some enemies just won't pathfind and cancel themselves when they shouldn't.
                // this Checksphere can PROBABLY stay?
                if (Physics.CheckSphere(pathfinding.path[0].worldPosition, 0.01f, LayerMask.GetMask("Physical"))) return;

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

        //This is a fix for a glitch involving origins.
        targetPos.y += 1f;

        // Add a bit of extra bump so the imp goes out more.
        Vector3 difference = targetPos - spriteRender.transform.position;

        // Start an interp here.
        attackInterp = new QuickInterpVec3(spriteRender.transform.position, targetPos - (difference.normalized) + (difference.normalized * attackBounce), attackSpeed, true);
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
                            stateObject.DamagePlayer((int)(baseDamage * damageDirection * stateObject.GetDamageModifier()), name);

                        }

                    }

                    // Reverse it with a new attack interp here, overriding the other one.
                    retreating = true;
                    attackInterp = new QuickInterpVec3(spriteRender.transform.position, prevPosition, attackSpeed, true);

                }
            }

            // Otherwise, keep interping.
            attackInterp.InterpUpdate();
        }
    }

}