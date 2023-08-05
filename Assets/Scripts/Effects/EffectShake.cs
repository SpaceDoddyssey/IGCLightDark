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
        float fadeAffector = 20f;
        while (fadeAffector > 2)
        {
            if (fadeAffector % 1 == 0) 
            {
                float randomScalarX = Random.Range(-1.3f, 1.3f);
                float randomScalarY = Random.Range(-1.3f, 1.3f);
                float randomScalarZ = Random.Range(-1.3f, 1.3f);
                // TODO: Fix this because enemies' positions are being moved I think, not their sprite billboards
                transform.localPosition = new Vector3(oldPos.x + (fadeAffector * randomScalarX * 0.0008f * damage), oldPos.y + (fadeAffector * randomScalarY * 0.0008f * damage), oldPos.z +(fadeAffector * randomScalarZ * 0.0008f * damage));


            }

            fadeAffector -= 1.0f;

            yield return new WaitForSecondsRealtime(.016f);

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


