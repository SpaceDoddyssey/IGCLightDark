using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool smoothTransition = true;
    public float transitionSpeed = 10f;
    public float transitionRotationSpeed = 500f;

    //Private variables
    Vector3 targetGridPos, prevTargetGridPos, targetRotation;

    // Start is called before the first frame update
    void Start() {
        targetGridPos = Vector3Int.RoundToInt(transform.position);
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
    public void MoveForward(){
        if (AtRest) {
            prevTargetGridPos = targetGridPos; 
            targetGridPos += transform.forward * scalar; 
        }
    }
    public void MoveBackward(){
        if (AtRest) { 
            prevTargetGridPos = targetGridPos;
            targetGridPos -= transform.forward * scalar; 
        }
    }
    public void MoveLeft(){
        if (AtRest) { 
            prevTargetGridPos = targetGridPos;
            targetGridPos -= transform.right * scalar; 
        }
    }
    public void MoveRight(){
        if (AtRest) { 
            prevTargetGridPos = targetGridPos;
            targetGridPos += transform.right * scalar; 
        }
    }
    public void RotateLeft(){ 
        if (AtRest) { 
            prevTargetGridPos = targetGridPos;
            targetRotation -= Vector3.up * 90f; 
        }
    }
    public void RotateRight(){ 
        if (AtRest) {
            prevTargetGridPos = targetGridPos; 
            targetRotation += Vector3.up * 90f; 
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
        Debug.Log(other.gameObject.name);
        if(other.gameObject.name == "block"){
            (targetGridPos, prevTargetGridPos) = (prevTargetGridPos, targetGridPos); //Swaps the two values, sending you back to where you started
        }
    }
}
