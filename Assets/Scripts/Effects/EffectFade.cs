using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFade : MonoBehaviour
{
    public void FadeIn(float time = 1.0f)
    {
        StartCoroutine("FadeInRoutine", time);
    }

    private IEnumerator FadeInRoutine(float time)
    {

        float totalTime = time;
        time = 0f;
        GameObject spriteObj = transform.GetChild(0).GetChild(0).gameObject;
        SpriteRenderer spriteRenderer = spriteObj.GetComponent<SpriteRenderer>();

        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
        float alpha;
        while ((time / totalTime) < 1f)
        {
            time += Time.fixedDeltaTime;
            alpha = Mathf.Clamp01(time / totalTime);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
            yield return new WaitForFixedUpdate();
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
        GameObject spriteObj = transform.GetChild(0).GetChild(0).gameObject;
        SpriteRenderer spriteRenderer = spriteObj.GetComponent<SpriteRenderer>();

        if (death)
        {
            GetComponent<EnemyScript>().enabled = false;
            spriteObj.GetComponent<Animator>().enabled = false;
        }

        while (time >= 0.0f)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, time / totalTime);
            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        if (death)
        {
            Destroy(gameObject);
        }

        spriteRenderer.enabled = false;

    }
}
