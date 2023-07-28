using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActionText : MonoBehaviour
{
    public float fadeTime;

    private string _text;
    public string Text
    {
        get
        {
            return _text;
        }
        set
        {
            _text = value;
            interp = new QuickInterp(1.0f, 0.0f, fadeTime, true);
        }
    }
    private TextMeshProUGUI textObj;
    private QuickInterp interp;

    void Start()
    {
        textObj = GetComponent<TextMeshProUGUI>();
        interp = new QuickInterp(1.0f, 0.0f, fadeTime, true);
    }

    void Update()
    {
        textObj.text = Text;
        interp.InterpUpdate();
        textObj.color = new Color(textObj.color.r, textObj.color.g, textObj.color.b, interp.status);
        if (interp.isDone)
        {
            Destroy(this.transform.gameObject);
        }
    }
}
