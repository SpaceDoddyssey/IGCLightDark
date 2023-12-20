using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using FMODUnity;
using Unity.VisualScripting;

public class TitleButton : MonoBehaviour
{
    public float fadeOutTime;
    private GameObject black;
    private bool pressed;

    [DoNotSerialize]
    public bool introSkip = false;

    // Start is called before the first frame update
    void Start()
    {
        black = GameObject.Find("FadeToBlack");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && !pressed)
        {
            if (introSkip)
            {
                OnClick();
            }
            else
            {
                black.GetComponent<Animator>().Play("idle");
                black.GetComponent<TitleAnimationEnable>().EnableAllAnims();
                black.GetComponent<TitleAnimationEnable>().PlayDaMusic();
            }



        }
    }

    public void OnClick()
    {
        pressed = true;
        black.GetComponent<Animator>().Play("FadeToBlack");
        black.GetComponent<TitleAnimationEnable>().decreasing = true;

        StartCoroutine("Timer");
    }

    IEnumerator Timer()
    {

        yield return new WaitForSeconds(7f);
        black.GetComponent<TitleAnimationEnable>().KillInstances();
        SceneManager.LoadScene("Level1");
    }


}
