using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectShake : MonoBehaviour
{
   private Vector3 oldPos;

   public void DoShake(int damage, bool player = false)
   {
        oldPos = transform.localPosition;
        StartCoroutine(Shake(damage, player));
   }

    private IEnumerator Shake(int damage, bool player = false)
    {
        oldPos = transform.localPosition;
        damage /= 7;
        float fadeAffector = 120f;
        while (fadeAffector > 2)
        {
            if (fadeAffector % 10 == 0) 
            {
                float randomScalarX = Random.Range(-1.3f, 1.3f);
                float randomScalarY = Random.Range(-1.3f, 1.3f);
                transform.localPosition = new Vector3(oldPos.x + (fadeAffector * randomScalarX * 0.0002f * damage), oldPos.y + (fadeAffector * randomScalarY * 0.0002f * damage), oldPos.z);


            }

            fadeAffector -= 1.0f;

            yield return new WaitForEndOfFrame();

        }

        if (player)
        {
            transform.localPosition = new Vector3(0, 0, transform.localPosition.z);
        }
        else
        {
            transform.localPosition = oldPos;
        }

    }
}


