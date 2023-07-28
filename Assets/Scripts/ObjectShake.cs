using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectShake : MonoBehaviour
{
   private Vector3 oldPos;

   public void DoShake(int damage)
   {
        oldPos = transform.localPosition;
        StartCoroutine("Shake", damage);
   }


    private IEnumerator Shake(int damage)
    {
        oldPos = transform.localPosition;
        damage /= 7;
        float fadeAffector = 120f;
        while (fadeAffector > 2)
        {
            if (fadeAffector % 20 == 0) 
            {
                float randomScalarX = Random.Range(-1.3f, 1.3f);
                float randomScalarY = Random.Range(-1.3f, 1.3f);
                transform.localPosition = new Vector3(oldPos.x + (fadeAffector * randomScalarX * 0.0005f * damage), oldPos.y + (fadeAffector * randomScalarY * 0.0005f * damage), oldPos.z);


            }

            fadeAffector -= 1.0f;

            yield return new WaitForEndOfFrame();

        }

        transform.localPosition = oldPos;

    }
}


