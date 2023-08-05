using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NullBar : MonoBehaviour
{
    [SerializeField] private GameObject segment;
    [SerializeField] private List<GameObject> segments;
    [SerializeField] private GameState stateObject;
    private int maxNumElements = 4;
    private Vector3 originalPos;
    private Vector3 originalCameraPos;
    private new GameObject camera;

    public float fadeInTime = 0.2f;
    public float fadeOutTime = 0.2f;

    private GameObject parentBorder;

    // Start is called before the first frame update
    void Start()
    {
        stateObject = GameObject.Find("Game World Manager").GetComponent<GameState>();
        camera = GameObject.Find("Main Camera");
        originalPos = transform.parent.transform.localPosition;
        originalCameraPos = camera.transform.localPosition;
        parentBorder = transform.parent.gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (segments.Count == 2)
        {
            transform.parent.transform.localPosition = new Vector3(originalPos.x + Random.Range(1f, 3f), originalPos.y + Random.Range(1f, 3f), 0f);

        }

        if (segments.Count > 2 && Time.frameCount % 2 == 0)
        {
            transform.parent.transform.localPosition = new Vector3(originalPos.x + Random.Range(2f, 5f), originalPos.y + Random.Range(2f, 5f), 0f);
            camera.transform.localPosition = new Vector3(originalCameraPos.x + (Random.Range(2f, 5f) * 0.01f), originalCameraPos.y + (Random.Range(2f, 5f) * 0.01f), originalCameraPos.z + (Random.Range(2f, 5f) * 0.01f));

        }
    }

    public void OnClockTwelve()
    {
        if (!isActiveAndEnabled) return;

        if (stateObject.polarity == 0)
        {
            if (segments.Count == 0)
                parentBorder.GetComponent<EffectFadeNonEnemy>().FadeIn(fadeInTime);

            if (segments.Count < maxNumElements)
            {
                var currentSegment = Instantiate(segment, transform);
                currentSegment.transform.SetAsFirstSibling();
                segments.Insert(0, currentSegment);


                if (segments.Count > maxNumElements - 1)
                    stateObject.StartCoroutine("PlayerDeath");
                else
                    currentSegment.GetComponent<EffectFadeNonEnemy>().FadeIn(fadeInTime);
            }

        }
        else if (stateObject.polarity != 0)
        {
            if (segments.Count > 0)
            {
                GameObject last = segments.First();
                segments.Remove(last);
                last.GetComponent<EffectFadeNonEnemy>().FadeOut(true, fadeOutTime);
            }

            if (segments.Count == 0)
            {
                parentBorder.GetComponent<EffectFadeNonEnemy>().FadeOut(false, fadeOutTime);
            }
        }
    }
}
