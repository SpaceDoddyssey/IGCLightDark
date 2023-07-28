using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NullBar : MonoBehaviour
{
    [SerializeField] private GameObject segment;
    [SerializeField] private List<GameObject> segments;
    private GameState stateObject;
    private int maxNumElements = 4;
    private int numElements = 0;

    // Start is called before the first frame update
    void Start()
    {
        stateObject = GameObject.Find("Game World Manager").GetComponent<GameState>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClockTwelve()
    {
        if (stateObject.polarity == 0)
        {
            if (segments.Count < 4)
            {
                var currentSegment = Instantiate(segment, transform);
                segments.Add(currentSegment);

                if (segments.Count > 3)
                    stateObject.PlayerDeath();
            }

        }
        else if (stateObject.polarity != 0)
        {
            if (segments.Count > 0)
            {
                GameObject last = segments.Last();
                segments.Remove(last);
                Destroy(last);
            }
        }
    }
}
