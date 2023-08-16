using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EffectFadeToFromBlack : MonoBehaviour
{
    private bool isFading;

    // Start is called before the first frame update
    void Start()
    {

    }

    public IEnumerator Fade(float fadeDuration = 1.0f, float targetAlpha = 1.0f)
    {

        float fadeTime = fadeDuration;
        float baseAlpha = GetAlpha();

        while (fadeTime >= 0)
        {
            fadeTime -= 1 * Time.unscaledDeltaTime;
            float lerped = Mathf.Lerp(targetAlpha, baseAlpha, fadeTime / fadeDuration);
            SetAlpha(lerped);
            yield return new WaitForSecondsRealtime(1 / Application.targetFrameRate);
        }
        SetAlpha(targetAlpha);
    }

    public float GetAlpha()
    {
        return GetComponent<Image>().color.a;
    }

    public void SetAlpha(float alpha)
    {
        Color c = GetComponent<Image>().color;
        GetComponent<Image>().color = new Color(c.r, c.g, c.b, alpha);
    }

    
}
