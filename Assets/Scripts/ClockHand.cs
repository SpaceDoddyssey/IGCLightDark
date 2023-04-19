using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockHand : MonoBehaviour
{
    // The at-rest speed of the clock, per second, without any player input.
    public float idleSpeed = 0.02f;

    private bool isTicking = false;
    private Vector3 rot = new Vector3(0f, 0f, 0f);

    // Start is called before the first frame update
    private void Start()
    {
        ToggleTick();
    }

    // Update is called once per frame
    private void Update()
    {
        // Rotate clock hand
        rot.z += idleSpeed * Time.deltaTime * -360f;
        CheckClockOverflow();

        transform.rotation = Quaternion.Euler(rot);
        
    }

    private void CheckClockOverflow()
    {
        if (rot.z < -360f)
        {
            rot.z += 360f;
            // Currently this doesn't catch if a player's actions are so big that they go beyond multiples of -360, like -720,
            // requiring the clock to strike twice.

            print("Clock strikes twelve!");
        }
    }

    public void ToggleTick()
    {
        isTicking = !isTicking;
    }

    // Rotates the clock hand instantly by a certain fraction.
    // Should ONLY be used by the world manager.
    public void RotateClockHand(float fraction)
    {
        // This gives the player a single frame before the clock recognizes it's potentially
        // gone past 12 o' clock in the update function.
        rot.z += -360f * fraction;
    }
    
}
