using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ClockGlow : MonoBehaviour
{
    public UnityEngine.Rendering.Universal.Light2D clockLight;

    void Awake() {
        clockLight = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
    }

    public void Glow() {
        StartCoroutine(Glowroutine());
    }

    IEnumerator Glowroutine () { //get it because it's like a coroutine but it glows so its a glowroutine
        float glowTime = 1f; //time to go from dark->bright and vice versa
        float maxIntensity = 30f;
        float smoothness = 0.01f;

        while (clockLight.intensity < maxIntensity) {
            clockLight.intensity += smoothness * maxIntensity / glowTime;
            yield return new WaitForSeconds(smoothness);
        }
        while (clockLight.intensity > 0) {
            clockLight.intensity -= smoothness * maxIntensity / glowTime;
            yield return new WaitForSeconds(smoothness);
        }

        clockLight.intensity = 0;
    }
}
