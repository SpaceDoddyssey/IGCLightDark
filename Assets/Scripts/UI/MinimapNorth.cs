using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapNorth : MonoBehaviour
{
    Vector3 basePosition;
    // Start is called before the first frame update
    void Start()
    {
        basePosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = GameObject.Find("Player").transform.position;
        Vector3 newPosition = basePosition;
        newPosition.x += playerPosition.x;
        newPosition.z += playerPosition.z + 8;
        transform.position = newPosition;
    }
}
