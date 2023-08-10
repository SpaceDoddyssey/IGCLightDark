using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClockHand : MonoBehaviour
{
    // The at-rest speed of the clock, per second, without any player input.
    public float idleSpeed = 0.02f;
    public UnityEvent OnClockStrikesEvent;

    private bool isTicking = false;
    private QuickInterp handInterp;

    [SerializeField] private GameObject nullBarController;
    [SerializeField] private GameState stateObject;

    // Start is called before the first frame update
    private void Start()
    {
        ToggleTick();

    }

    // Update is called once per frame
    private void Update()
    {
        // Rotate clock hand
        stateObject.clockRotation.z += idleSpeed * Time.deltaTime * -360f * stateObject.GetSlowdownFactor();
        CheckClockOverflow();

        transform.localRotation = Quaternion.Euler(stateObject.clockRotation);
        
    }

    private void CheckClockOverflow()
    {
        if (stateObject.clockRotation.z < -360f)
        {
            stateObject.clockRotation.z += 360f;
            // Currently this doesn't catch if a player's actions are so big that they go beyond multiples of -360, like -720,
            // requiring the clock to strike twice.

            Animator clock = transform.parent.GetComponent<Animator>();
            clock.Play("clock_glow");

            print("Clock strikes twelve!");
            OnClockStrikesEvent.Invoke();
            FMODUnity.RuntimeManager.PlayOneShot("event:/World/Clock/clk_strike");

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
        stateObject.clockRotation.z += -360f * fraction;
        Animator hand = GetComponent<Animator>();
        if (hand.isActiveAndEnabled)
            hand.Play("hand_glow");
        FMODUnity.RuntimeManager.PlayOneShot("event:/World/Clock/clk_tick");
    }

    public void LinkEvents()
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            OnClockStrikesEvent.AddListener(g.GetComponent<EnemyScript>().OnClockTwelve);
        }
        OnClockStrikesEvent.AddListener(nullBarController.GetComponent<NullBar>().OnClockTwelve);
    }

    // When the clock is rotated by any amount, there should be a "target" rotation.
    // If that target rotation is less than -360 degrees,then add 360 degrees to both the clock's current rotation AND the target rotation,
    // then set the interp. But to the target, ADD the time that would have been added from ticking, and only re-enable ticking when the lerp is done.

    // Gotta be a better way to do this.
    
}
