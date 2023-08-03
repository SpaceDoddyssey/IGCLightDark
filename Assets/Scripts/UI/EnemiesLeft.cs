using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemiesLeft : MonoBehaviour
{
    public bool isDark = false;
    private TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        int count = 0;

        GameObject list = isDark ? GameObject.Find("Dark Enemies") : GameObject.Find("Light Enemies");
        if (list != null)
        {
            foreach (EnemyScript g in list.transform.GetComponentsInChildren<EnemyScript>())
            {
                count++;
            }
            text.text = count.ToString();
        }

        if (count == 0) { text.color = Color.red; }
    }
}
