using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{
    public bool skipTutorial;
    public ClockHand hand;
    public PolarityBarTicker ticker;
    public float playerHealth = 0;
    public ItemType curHeldItem = ItemType.None;
    public Image curHeldItemSprite;
    public Sprite[] itemSprites; //Make sure to add to this if you add a new item

    public GameObject player;

    public float playerMaxAbsoluteHealth;
    public int polarity;

    public Vector3 clockRotation = new Vector3(0f, 0f, 0f);

    public GameObject ActionTextPrefab;
    public GameObject dialogueWindow;

    [SerializeField] private GameObject glass1;
    [SerializeField] private GameObject glass2;
    [SerializeField] private new ParticleSystem particleSystem;

    [SerializeField] private GameObject deathScreen;
    [SerializeField] private GameObject endScreen;

    private EffectFadeToFromBlack fade;

    private AStarGrid grid;

    private GameObject gradient;

    private GameObject clock;
    private GameObject polarityBar;
    private GameObject healthBar;
    private GameObject minimap;
    private GameObject nullBar;

    private GameObject darkEnemies;
    private GameObject lightEnemies;

    [SerializeField] private GameObject finalTutorialBlock;

    public bool isPlayerDead = false;
    public bool nullDeath = false;
    private bool isPlayerDying = false;

    private bool allEnemiesDead = false;

    private bool gameClear = false;
    private bool deathCooldown;

    private bool canRestart = false;

    // TUTORIAL STUFF
    [SerializeField]
    private Transform
        minimapEnableSpot,
        PolarityBarEnableSpot,
        healthBarEnableSpot,
        clockEnableSpot,
        NullBarEnableSpot,
        finalEnableSpot,
        gameClearSpot;

    // SOUND STUFF
    private FMOD.Studio.EventInstance deathSound;

    [SerializeField]
    private List<string> tips;


    private string[] goodWords =
    {
        "LOVES",
        "ADORES",
        "CHERISHES",
        "COMFORTS",
        "AIDS",
        "DESIRES",
        "INDULGES",
        "MENDS",
        "HEALS",
        "RESTORES",
        "ACCEPTS"
    };

    private string[] badWords =
    {
        "HATES",
        "SCORNS",
        "SPURNS",
        "LOATHES",
        "DETESTS",
        "ABHORS",
        "CURSES",
        "ANTAGONIZES",
        "TRIVIALIZES",
        "NEUTRALIZES",
    };


    IEnumerator RestartTimer()
    {
        yield return new WaitForSecondsRealtime(1);
        canRestart = true;
    }

    void Awake()
    {
        StartCoroutine(RestartTimer());

        Time.timeScale = 1f;

        deathSound = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Death/pl_dth");
        Application.targetFrameRate = 60;
        //curHeldItemSprite = GameObject.Find("ItemSprite").GetComponent<Image>();
        //curHeldItemSprite.enabled = false;

        fade = GameObject.Find("FadeToFromBlack").GetComponent<EffectFadeToFromBlack>();
        fade.SetAlpha(1.0f);
        fade.StartCoroutine(fade.Fade(1.2f, 0.0f));


        if (PlayerPrefs.GetInt("SkipTutorial") == 0) 
        {
            skipTutorial = false;
        }
        else
        {
            skipTutorial = true;
        }

        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Player/Spawn/pl_spawn", gameObject);

        darkEnemies = GameObject.Find("Dark Enemies");
        lightEnemies = GameObject.Find("Light Enemies");
        clock = GameObject.Find("Clock");
        polarityBar = GameObject.Find("PolarityBar");
        healthBar = GameObject.Find("HealthBar");
        minimap = GameObject.Find("MinimapMask");
        nullBar = GameObject.Find("NullBarBG");
        curHeldItemSprite = GameObject.Find("ItemSlot").GetComponent<Image>();
        curHeldItemSprite.enabled = false;

        grid = GetComponent<AStarGrid>();


        if (!skipTutorial)
        {
            clock.SetActive(false);
            minimap.SetActive(false);
            polarityBar.SetActive(false);
            healthBar.SetActive(false);
            darkEnemies.SetActive(false);
            lightEnemies.SetActive(false);
            nullBar.SetActive(false);

            player.transform.position = GameObject.Find("TutorialStart").transform.localPosition;
        }
        else
        {
            hand.LinkEvents();
        }

    }


    void Update()
    {
        if (isPlayerDead && !isPlayerDying)
        {
            StartCoroutine("PlayerDeath", nullDeath);
            isPlayerDying = true;
        }

        if (CheckSharedGridPosition(player.transform, minimapEnableSpot))
        {
            if (minimap.activeSelf == false)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/World/Clock/clk_strike");
                minimap.SetActive(true);
            }
        }

        if (CheckSharedGridPosition(player.transform, PolarityBarEnableSpot))
        {
            if (polarityBar.activeSelf == false)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/World/Clock/clk_strike");
                polarityBar.SetActive(true);
            }
        }


        if (GameObject.Find("TutRoom1Enemies").transform.childCount == 0 && GameObject.Find("PolarityTutDoor").GetComponent<DoorScript>().IsOpen == false)
        {
            GameObject.Find("PolarityTutDoor").GetComponent<DoorScript>().Open();
        }
    

        if (CheckSharedGridPosition(player.transform, healthBarEnableSpot))
        {
            if (healthBar.activeSelf == false)
            {
                healthBar.SetActive(true);
                FMODUnity.RuntimeManager.PlayOneShot("event:/World/Clock/clk_strike");
            }

        }


        if (CheckSharedGridPosition(player.transform, clockEnableSpot))
        {
            if (clock.activeSelf == false)
            {
                clock.SetActive(true);
            }

        }

        if (CheckSharedGridPosition(player.transform, NullBarEnableSpot))
        {
            if (nullBar.activeSelf == false)
            {
                polarityBar.transform.Find("PolarityBarTicker").GetComponent<PolarityBarTicker>().SetTickerOffset(polarity * -1);
                nullBar.SetActive(true);
                clock.transform.Find("ClockHand").GetComponent<ClockHand>().OnClockStrikesEvent.AddListener(nullBar.transform.GetChild(0).GetComponent<NullBar>().OnClockTwelve);
            }

        }


        if ((CheckSharedGridPosition(player.transform, finalEnableSpot) && GameObject.Find("FinalTutDoor").GetComponent<DoorScript>().IsOpen == false))
        {
            GameObject.Find("FinalTutDoor").GetComponent<DoorScript>().Open();
            finalTutorialBlock.SetActive(true);
            darkEnemies.SetActive(true);
            lightEnemies.SetActive(true);
            hand.LinkEvents();
        }

        if (Input.GetKeyDown("r"))
        {
            if (isPlayerDead && !deathCooldown) return;
            if (!canRestart) return;
            FMOD.Studio.Bus mainBus = RuntimeManager.GetBus("bus:/");
            mainBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
            deathSound.release();
            PlayerPrefs.SetInt("SkipTutorial", 1);
            SceneManager.LoadScene("Level1");
        }

        //if (Input.GetKeyDown("x"))
        //{
        //    FMOD.Studio.Bus mainBus = RuntimeManager.GetBus("bus:/");
        //    mainBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        //    deathSound.release();
        //    PlayerPrefs.SetInt("SkipTutorial", 0);
        //    SceneManager.LoadScene("Level1");
        //}



        //if (Input.GetKeyDown("o")) {
        //    DisplayDialogueWindow(dialogueWindow);
        //} else if (Input.GetKeyDown("p")) {
        //    CloseDialogueWindow(dialogueWindow);
        //}

        if (Input.GetKeyDown("j")){
            if(curHeldItem == ItemType.None){
                print("No item to use!");
            } else {
                if(Item.Use(curHeldItem)){ //If the item is successfully used
                    curHeldItem = ItemType.None;
                    curHeldItemSprite.enabled = false;
                }
            } 
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }


        if (polarity == 0 && !particleSystem.isPaused)
        {
            particleSystem.Pause();
        }
        else if (polarity != 0 && particleSystem.isPaused)
        {
            particleSystem.Play();
        }


        if (!allEnemiesDead && darkEnemies.transform.childCount == 0 && lightEnemies.transform.childCount == 0)
        {
            allEnemiesDead = true;
            GameObject.Find("RedDoor").GetComponent<DoorScript>().Open();
        }

        if (CheckSharedGridPosition(player.transform, gameClearSpot) && gameClear == false)
        {
            gameClear = true;
            Time.timeScale = 0;

            StartCoroutine(GameClear());
        }



    }



    [SerializeField]
    private List<float> enemyDamageModifiers = new List<float>()
    {
        0f,
        50f,
        75f,
        125f,
        200f
    };

    public float GetDamageModifier()
    {
        return enemyDamageModifiers[Mathf.Abs(polarity)];
    }

    [SerializeField]
    private List<float> damageToEnemyValues = new List<float>()
    {
        0f,
        15f,
        20f,
        30f,
        60f
    };

    public float GetDamageToEnemy()
    {
        return damageToEnemyValues[Mathf.Abs(polarity)];
    }

    [SerializeField]
    private List<float> slowdownFactors = new List<float>()
    {
        100f,
        100f,
        90f,
        80f,
        75f,
    };

    public float GetSlowdownFactor()
    {
        return slowdownFactors[Mathf.Abs(polarity)] / 10f;
    }

    public float GetGradientSpeed()
    {
        switch (Mathf.Abs(polarity))
        {
            case 0:
                return 1f;
            case 1:
                return 1.25f;
            case 2:
                return 1.3f;
            case 3:
                return 2f;
            case 4:
                return 3f;
            default:
                return 1f;
        }
    }


    public void ShiftPolarity(int offset)
    {
        if (ticker != null && ticker.enabled && polarityBar.activeSelf)
        {
            ticker.SetTickerOffset(offset);
        }
        if (hand != null && hand.enabled)
        {
            hand.RotateClockHand(0.20f * Mathf.Abs(offset));
        }
    }

    public void TurnClock(float amount, bool advancing = false)
    {
        if (hand.isActiveAndEnabled)
            hand.RotateClockHand(amount, advancing);
    }

    public void DamagePlayer(float damage, string name)
    {
        playerHealth += damage;
        string desiredString = name + " " + (name == "Lightener" ? findGoodWords() : findBadWords()) + " Player for " + damage + " damage!";

        // Put this on the text on the screen.

        //Transform textSpot = GameObject.Find("TextSpot").transform;
        //GameObject findExistingText = GameObject.Find("ActionText(Clone)");
        //if (findExistingText != null)
        //{
        //    string existingText = findExistingText.GetComponent<ActionText>().Text;
        //    existingText = existingText.Insert(0, desiredString + "\n");
        //    findExistingText.GetComponent<ActionText>().Text = existingText;
        //}
        //else
        //{
        //    GameObject text = Instantiate(ActionTextPrefab, textSpot);
        //    text.GetComponent<ActionText>().Text = desiredString;
        //}

        player.transform.Find("Main Camera").GetComponent<EffectShake>().DoShake(damage, true);

        //float ratio = (0f + (float)Mathf.Abs(playerHealth) / (float)Mathf.Abs(playerMaxAbsoluteHealth)) * -2f;
        //healthBar.transform.localRotation = Quaternion.Euler(0f, 0f, 0f + ratio);

        if (Mathf.Abs(polarity) >= 3)
            healthBar.transform.GetChild(0).GetComponent<Animator>().Play("damage2");
        else
            healthBar.transform.GetChild(0).GetComponent<Animator>().Play("damage1");

        if (playerHealth > playerMaxAbsoluteHealth || playerHealth < playerMaxAbsoluteHealth * -1)
        {
            playerHealth = Mathf.Clamp(playerHealth, playerMaxAbsoluteHealth * -1, playerMaxAbsoluteHealth);
            isPlayerDead = true;
        }


    }

    public void AcquireRandItem(){
        if(curHeldItem != ItemType.None) { return; }
        var itemType = Item.RandomItemData();
        curHeldItem = itemType; 
        curHeldItemSprite.enabled = true;
        curHeldItemSprite.sprite = itemSprites[(int)itemType];
    }


    public string findGoodWords()
    {
        return goodWords[(int)Random.Range(0f, goodWords.Length - 1)];
    }

    public string findBadWords()
    {
        return badWords[(int)Random.Range(0f, goodWords.Length - 1)];
    }

    public void PrintEnemyDamageText(int amount, string enemyName)
    {
        string desiredString = "Player hits " + enemyName + " for " + amount + " damage!";

        // Put this on the text on the screen.

        Transform textSpot = GameObject.Find("TextSpot").transform;
        GameObject findExistingText = GameObject.Find("ActionText(Clone)");
        if (findExistingText != null)
        {
            string existingText = findExistingText.GetComponent<ActionText>().Text;
            existingText = existingText.Insert(0, desiredString + "\n");
            findExistingText.GetComponent<ActionText>().Text = existingText;
        }
        else
        {
            GameObject text = Instantiate(ActionTextPrefab, textSpot);
            text.GetComponent<ActionText>().Text = desiredString;
        }
    }

    IEnumerator PlayerDeath(bool nullDeath)
    {

        yield return new WaitForSeconds(0.1f);
        FMOD.Studio.Bus mainBus = RuntimeManager.GetBus("bus:/");
        mainBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);

        Time.timeScale = 0f;

        print("Threshhold broken! Game over!");

        deathSound.start();
        yield return new WaitForSecondsRealtime(0.03f);


        GameObject glass;

        if (Random.Range(0f, 1f) < .5f) glass = glass1;
        else glass = glass2;

        player.transform.Find("Main Camera").GetComponent<EffectShake>().DoShake(3, true);

        GameObject glassObj = GameObject.Instantiate(glass, polarityBar.transform.GetChild(0));
        glassObj.transform.localPosition = new Vector3(Random.Range(-70f, 70f), glassObj.transform.localPosition.y, glassObj.transform.localPosition.z);

        GameObject.Find("Player").GetComponent<PlayerController>().enabled = false;
        GameObject.Find("Player").GetComponent<PlayerInput>().enabled = false;

        PlayerPrefs.SetInt("SkipTutorial", 1);
        yield return new WaitForSecondsRealtime(1f);
        StartCoroutine(fade.Fade(4f, .95f));
        yield return new WaitForSecondsRealtime(2f);
        GameObject death = Instantiate(deathScreen, GameObject.Find("Canvas").transform);
        death.transform.SetAsLastSibling();
        if (nullDeath)
            death.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text += tips[0];
        else
            death.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text += GetTip();

        yield return new WaitForSecondsRealtime(1f);
        deathCooldown = true;
    }


    private IEnumerator GameClear()
    {
        Time.timeScale = 0;
        FMOD.Studio.Bus mainBus = RuntimeManager.GetBus("bus:/");
        mainBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);

        yield return new WaitForSecondsRealtime(2f);

        FMODUnity.RuntimeManager.PlayOneShot("event:/mus_theme_rev");

        StartCoroutine(fade.Fade(5f, 1f));
        yield return new WaitForSecondsRealtime(5f);

        GameObject end = Instantiate(endScreen, GameObject.Find("Canvas").transform);
        end.transform.SetAsLastSibling();



    }

    string GetTip()
    {
        return tips[Random.Range(0, tips.Count)];
    }

    void PauseGame() {
        Time.timeScale = 0;
    }

    void UnPauseGame() {
        Time.timeScale = 1;
    }

    void DisplayDialogueWindow (GameObject dialogueWindow) {
        PauseGame();
        dialogueWindow.SetActive(true);
    }

    void CloseDialogueWindow(GameObject dialogueWindow) {
        UnPauseGame();
        dialogueWindow.SetActive(false);
    }

    bool CheckSharedGridPosition(Transform a, Transform b)
    {
        return (grid.AStartNodeFromWorldPoint(a.position) == grid.AStartNodeFromWorldPoint(b.position));
    }

}
