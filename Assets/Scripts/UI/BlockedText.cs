using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockedText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Die");
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + (10f * Time.deltaTime));
    }

    IEnumerator Die()
    {
        EffectFadeToFromBlack fade = GetComponent<EffectFadeToFromBlack>();
        


        yield return fade.StartCoroutine(fade.Fade(0.3f, 0f));
        Destroy(gameObject);
        yield return null;
    }
}
