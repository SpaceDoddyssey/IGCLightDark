using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
    public KeyCode forward = KeyCode.W;
    public KeyCode back  = KeyCode.S;
    public KeyCode left  = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode turnLeft = KeyCode.Q;
    public KeyCode turnRight = KeyCode.E;

    public KeyCode addDark = KeyCode.Alpha1;
    public KeyCode addLight = KeyCode.Alpha3;

    PlayerController controller;
    WorldState worldManager;

    private void Awake(){
        controller = GetComponent<PlayerController>();
    }

    private void Update(){
        if (Input.GetKeyDown(forward)) controller.MoveForward();
        if (Input.GetKeyDown(back)) controller.MoveBackward();
        if (Input.GetKeyDown(left)) controller.MoveLeft();
        if (Input.GetKeyDown(right)) controller.MoveRight();
        if (Input.GetKeyDown(turnLeft)) controller.RotateLeft();
        if (Input.GetKeyDown(turnRight)) controller.RotateRight();

        if (Input.GetKeyDown(addDark)) worldManager.ChangePolarity(-1);
        if (Input.GetKeyDown(addLight)) worldManager.ChangePolarity(1);
    }
}