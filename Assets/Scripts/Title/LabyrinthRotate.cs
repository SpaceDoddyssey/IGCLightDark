using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabyrinthRotate : MonoBehaviour
{
    [SerializeField] 
    private float circleRotation;
    private Vector3 rotate = new Vector3(0, 0, 0);

    // Update is called once per frame
    void Update()
    {

        rotate.z -= circleRotation * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(rotate);
    }
}
