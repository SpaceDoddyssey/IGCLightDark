using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeathScreenWaver : MonoBehaviour
{
    [SerializeField] private float waverAmount;

    void Start()
    {
        transform.localRotation = Quaternion.Euler(0, Mathf.Sin(Time.realtimeSinceStartup) * waverAmount, 0);
        GetComponent<EffectFadeNonEnemy>().FadeIn(4f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.Euler(0, Mathf.Sin(Time.realtimeSinceStartup) * waverAmount, 0);
    }

}
