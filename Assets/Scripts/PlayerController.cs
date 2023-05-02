using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{

    public bool smoothTransition = true;
    public float transitionSpeed = 10f;
    public float transitionRotationSpeed = 500f;


    //Private variables
    Vector3 targetGridPos, prevTargetGridPos, targetRotation;
    private GameState worldManager;

    // Start is called before the first frame update
    void Start() {

        worldManager = GameObject.Find("Game World Manager").GetComponent<GameState>();

        targetGridPos = Vector3Int.RoundToInt(transform.position);

        Tilemap tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        Debug.Log(tilemap);
        TileBase[] tileArray = tilemap.GetTilesBlock(new BoundsInt(-10, -10, -10, 20, 20, 20));
        //for (int index = 0; index < tileArray.Length; index++)
        //{
        //    Debug.Log(tileArray[index]);
        //}
    }

    // Update is called once per frame
    void FixedUpdate() {
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

    private float scalar = 2; //Right now the grid is scaled such that the player should move 2 unity units. That may change.
    //All move functions store where you were coming from so you can move back there if you hit an obstacle, and then set your target to the appropriate location
    public void MoveForward(){
        if (AtRest) {
            prevTargetGridPos = targetGridPos; 
            targetGridPos += transform.forward * scalar;
            worldManager.TurnClock(0.25f);
        }
    }
    public void MoveBackward(){
        if (AtRest) { 
            prevTargetGridPos = targetGridPos;
            targetGridPos -= transform.forward * scalar;
            worldManager.TurnClock(0.25f);
        }
    }
    public void MoveLeft(){
        if (AtRest) { 
            prevTargetGridPos = targetGridPos;
            targetGridPos -= transform.right * scalar;
            worldManager.TurnClock(0.25f);
        }
    }
    public void MoveRight(){
        if (AtRest) { 
            prevTargetGridPos = targetGridPos;
            targetGridPos += transform.right * scalar;
            worldManager.TurnClock(0.25f);
        }
    }
    public void RotateLeft(){
        if (AtRest)
        {
            targetRotation -= Vector3.up * 90f;
            worldManager.TurnClock(0.1f);
        }
    }
    public void RotateRight(){
        if (AtRest)
        {
            targetRotation += Vector3.up * 90f;
            worldManager.TurnClock(0.1f);
        }
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
        if(other.gameObject.name == "ph_block")
        {
            (targetGridPos, prevTargetGridPos) = (prevTargetGridPos, targetGridPos); //Swaps the two values, sending you back to where you started
        }
        else if(other.gameObject.tag == "Enemy"){
            if (other.gameObject.GetComponent<EnemyScript>().inPlayerDimension)
            {
                other.gameObject.GetComponent<EnemyScript>().TakeDamage((int)(10 * worldManager.GetDamageModifier()));
                (targetGridPos, prevTargetGridPos) = (prevTargetGridPos, targetGridPos);
            }
        }
    }
}
