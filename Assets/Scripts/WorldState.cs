using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour
{
    public int timeOnClock; //Replace this with the actual clock when the code is merged
    public int polarity {
        get; private set;
    }

    void Start(){
        polarity = 0;
    }

    public void changePolarity(int delta){
        if(polarity + delta < -100){
            polarity = -100;
        } else if(polarity + delta > 100){
            polarity = 100; 
        } else {
            polarity += delta;
        }

        Debug.Log("Polarity = " + polarity);
    }
}
