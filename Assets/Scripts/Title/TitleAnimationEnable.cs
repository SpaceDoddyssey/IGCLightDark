using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class TitleAnimationEnable : MonoBehaviour
{
    [SerializeField]
    private float speed = 1.0f;
    public bool decreasing = false;
    EventInstance song;
    EventInstance intro;
    // Start is called before the first frame update
    void Start()
    {
        song = RuntimeManager.CreateInstance("event:/mus_theme");
        intro = RuntimeManager.CreateInstance("event:/intro");
        intro.start();
    }

    private void Update()
    {
        song.setParameterByName("Speed", speed);
        if (decreasing) speed -= Time.deltaTime * 0.1f;
    }

    public void EnableAllAnims()
    {
        GameObject.Find("BGColor").GetComponent<Animator>().enabled = true;
        GameObject.Find("Maze").GetComponent<Animator>().enabled = true;
        GameObject.Find("EgoType").GetComponent<Animator>().enabled = true;
        GameObject.Find("LabyrinthType").GetComponent<Animator>().enabled = true;
        GameObject.Find("CopyrightText").GetComponent<Animator>().enabled = true;
        GameObject.Find("PressAnyKeyText").GetComponent<Animator>().enabled = true;
        GameObject.Find("Maze").GetComponent<TitleButton>().introSkip = true;
    }

    public void PlayDaMusic()
    {
        intro.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        PLAYBACK_STATE state;
        song.getPlaybackState(out state);
        if (!(state == PLAYBACK_STATE.PLAYING))
        {
            FMOD.RESULT e = song.start();
            print(e);
        }

    }

    public void KillInstances()
    {
        song.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        song.release();
        intro.release();
    }
}
