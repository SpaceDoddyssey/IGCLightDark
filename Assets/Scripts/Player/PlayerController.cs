using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{

    public bool smoothTransition = true;
    public float transitionSpeed = 10f;
    public float transitionRotationSpeed = 500f;
    public float movementCost = 0.4f;
    public float turnCost = 0.1f;
    public float attackCost = 0.6f;

    private bool isMoving;
    [SerializeField] private GameObject blockedText;

    //Private variables
    Vector3 targetGridPos, prevTargetGridPos, targetRotation;
    private GameState gameState;

    // Start is called before the first frame update
    void Start() {

        gameState = GameObject.Find("Game World Manager").GetComponent<GameState>();

        Tilemap tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        TileBase[] tileArray = tilemap.GetTilesBlock(new BoundsInt(-10, -10, -10, 20, 20, 20));
        //for (int index = 0; index < tileArray.Length; index++)
        //{
        //    Debug.Log(tileArray[index]);
        //}

        Vector3Int.RoundToInt(transform.position);

        targetGridPos = Vector3Int.RoundToInt(transform.position);

    }

    // Update is called once per frame
    void FixedUpdate() {
    if (isMoving)
        HandleMovement();
    }

    void HandleMovement(){
        Vector3 targetPosition = targetGridPos;
    
        //Keep the rotation values in bound
        if(targetRotation.y > 270f && targetRotation.y < 361f) targetRotation.y = 0f;
        if(targetRotation.y < 0f) targetRotation.y = 270f;

        if(!smoothTransition){
            transform.position = targetPosition;
            transform.rotation = Quaternion.Euler(targetRotation);
        } else {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * transitionSpeed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * transitionRotationSpeed);
        }
    }


    private bool MoveCheck(Vector3 direction)
    {
        //return true;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, 2.0f);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.layer == 8 && hit.collider.tag != "Lever")
                return false;
        }
        return true;
    }


    private float scalar = 2; //Right now the grid is scaled such that the player should move 2 unity units. That may change.
    //All move functions store where you were coming from so you can move back there if you hit an obstacle, and then set your target to the appropriate location
    public void MoveForward(){
        if (AtRest) {
            if (!MoveCheck(transform.forward)) return;
            prevTargetGridPos = targetGridPos; 
            targetGridPos += transform.forward * scalar;
            gameState.TurnClock(movementCost * gameState.GetSlowdownFactor());
            isMoving = true;
        }
    }
    public void MoveBackward(){
        if (AtRest) {
            if (!MoveCheck(transform.forward * -1)) return;
            prevTargetGridPos = targetGridPos;
            targetGridPos -= transform.forward * scalar;
            gameState.TurnClock(movementCost * gameState.GetSlowdownFactor());
            isMoving = true;
        }
    }
    public void MoveLeft(){
        if (AtRest) {
            if (!MoveCheck(transform.right * -1)) return;
            prevTargetGridPos = targetGridPos;
            targetGridPos -= transform.right * scalar;
            gameState.TurnClock(movementCost * gameState.GetSlowdownFactor());
            isMoving = true;
        }
    }
    public void MoveRight(){
        if (AtRest) { 
            prevTargetGridPos = targetGridPos;
            if (!MoveCheck(transform.right)) return;
            targetGridPos += transform.right * scalar;
            gameState.TurnClock(movementCost * gameState.GetSlowdownFactor());
            isMoving = true;
        }
    }
    public void RotateLeft(){
        if (AtRest)
        {
            targetRotation -= Vector3.up * 90f;
            gameState.TurnClock(turnCost * gameState.GetSlowdownFactor());
            isMoving = true;
        }
    }
    public void RotateRight(){
        if (AtRest)
        {
            targetRotation += Vector3.up * 90f;
            gameState.TurnClock(turnCost * gameState.GetSlowdownFactor());
            isMoving = true;
        }
    }

    public void AttemptShiftPolarity(int offset)
    {
        if (Mathf.Abs(gameState.polarity) >= 4 && (Mathf.Sign(gameState.polarity) == Mathf.Sign(offset)))
        {
            return;
        }

        RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down, 2f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
        foreach (RaycastHit hit in hits)
        {
            EnemyScript e = hit.collider.GetComponent<EnemyScript>();
            if (e != null && gameState.polarity == 0)
            {
                if ((offset > 0 && e.homeWorld == EnemyScript.HomeWorld.Light) || (offset < 0 && e.homeWorld == EnemyScript.HomeWorld.Dark))
                {
                    Instantiate(blockedText, GameObject.Find("Canvas").transform);
                    return;

                }
            }


        }

        gameState.ShiftPolarity(offset);
    }

    bool AtRest {
        get {
            //Note to future self: Worry that allowing some variance here may cause the player to get slightly misaligned with the grid.
            //If that becomes an issue, start here to find a fix.
                if((Vector3.Distance(transform.position, targetGridPos) < 0.05f) &&
               (Vector3.Distance(transform.eulerAngles, targetRotation) < 0.05f)) {
                return true;
               } else { 
                return false;
            }
        }
    }

    void OnTriggerEnter(Collider other){
        //Debug.Log(other.gameObject.name);
        //Task: Clean up this whole system
        if(other.gameObject.layer == LayerMask.NameToLayer("Unwalkable"))
        {
            (targetGridPos, prevTargetGridPos) = (prevTargetGridPos, targetGridPos); //Swaps the two values, sending you back to where you started
        }
        LeverScript lever = other.gameObject.GetComponent<LeverScript>();
        if(lever != null){
            lever.Pull();
        }
        else if(other.gameObject.tag == "Enemy"){
            if (other.gameObject.GetComponent<EnemyScript>().inPlayerDimension)
            {
                other.gameObject.GetComponent<EnemyScript>().TakeDamage((int)(gameState.GetDamageToEnemy() * UnityEngine.Random.Range(0.90f, 1.10f)));
                (targetGridPos, prevTargetGridPos) = (prevTargetGridPos, targetGridPos);
            }
        } 
    }
}
