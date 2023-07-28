using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleButton : MonoBehaviour
{
    public float fadeOutTime;
    private QuickInterp interp;
    private Image black;

    // Start is called before the first frame update
    void Start()
    {
        black = GameObject.Find("FadeToBlack").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (interp != null)
        {
            interp.InterpUpdate();
            black.color = new Color(black.color.r, black.color.g, black.color.b, interp.status);
            if (interp.isDone) SceneManager.LoadScene("Level1");
        }
    }

    public void OnClick()
    {
        interp = new QuickInterp(0.0f, 1.0f, fadeOutTime, true);
        gameObject.GetComponent<Button>().interactable = false;
        black.enabled = true;
    }
}
