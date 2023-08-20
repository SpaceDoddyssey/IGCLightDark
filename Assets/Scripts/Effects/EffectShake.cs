using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectShake : MonoBehaviour
{
   private Vector3 oldPos;
   private GameState stateObject;

    private void Start()
    {
        stateObject = GameObject.Find("Game World Manager").GetComponent<GameState>();
    }

    public void DoShake(float damage, bool player = false)
   {
        if (!player)
            oldPos = new Vector3(0, transform.localPosition.y, 0);
        else
            oldPos = new Vector3(0, 0, -.781f);

        StartCoroutine(Shake(damage, player));
   }

    private IEnumerator Shake(float damage, bool player = false)
    {

        float fadeAffector = 20f;
        while (fadeAffector > 2)
        {
            if (fadeAffector % 1 == 0) 
            {
                float randomScalarX = Random.Range(-1.3f, 1.3f);
                float randomScalarY = Random.Range(-1.3f, 1.3f);
                float randomScalarZ = Random.Range(-1.3f, 1.3f);
                // TODO: Fix this because enemies' positions are being moved I think, not their sprite billboards
                if (player)
                {
                    float playerScalar = 800f;
                    transform.localPosition = new Vector3(
                        oldPos.x + (fadeAffector * randomScalarX * damage   / playerScalar), 
                        oldPos.y + (fadeAffector * randomScalarY * damage   / playerScalar), 
                        oldPos.z + (fadeAffector * randomScalarZ * damage   / playerScalar));
                }
                else
                {
                    float enemyScalar = 70f;
                    transform.localPosition = new Vector3(
                        oldPos.x + (fadeAffector * randomScalarX / enemyScalar),
                        oldPos.y + (fadeAffector * randomScalarY / enemyScalar),
                        oldPos.z + (fadeAffector * randomScalarZ / enemyScalar));
                }


            }

            fadeAffector -= 1.0f;

            yield return new WaitForSecondsRealtime(.016f);

        }

        transform.localPosition = oldPos;

    }
}


