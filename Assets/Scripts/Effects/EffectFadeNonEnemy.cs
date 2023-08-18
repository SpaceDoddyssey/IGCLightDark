using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class EffectFadeNonEnemy : MonoBehaviour
{
    public void FadeIn(float time = 1.0f)
    {
        StartCoroutine("FadeInRoutine", time);
    }

    private IEnumerator FadeInRoutine(float time)
    {
        
        float totalTime = time;
        time = 0f;
        Image image = GetComponent<Image>();
        image.enabled = true;

        image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
        float alpha;
        while ((time / totalTime) < 1f)
        {
            time += Time.unscaledDeltaTime;
            alpha = Mathf.Clamp01(time / totalTime);
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            yield return new WaitForSecondsRealtime(1 / Application.targetFrameRate);
        }

        yield return null;

    }

    public void FadeOut(bool death = false, float time = 1.0f) 
    {
        StartCoroutine(FadeOutRoutine(death, time));
    }

    private IEnumerator FadeOutRoutine(bool death, float time)
    {
        float totalTime = time;
        Image image = GetComponent<Image>();

        while (time >= 0.0f)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, time / totalTime);
            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        if (death)
        {
            Destroy(gameObject);
        }

        image.enabled = false;

    }
}
