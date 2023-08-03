using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameState : MonoBehaviour
{
    public bool isDebug;
    public ClockHand hand;
    public PolarityBarTicker ticker;
    public int playerHealth = 0;
    public ItemType curHeldItem = ItemType.None;
    public Image curHeldItemSprite;
    public Sprite[] itemSprites; //Make sure to add to this if you add a new item

    public GameObject player;

    public int playerMaxAbsoluteHealth = 100;
    public int polarity;

    public Vector3 clockRotation = new Vector3(0f, 0f, 0f);

    public GameObject ActionTextPrefab;
    public GameObject dialogueWindow;

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


    //TUTORIAL STUFF
    [SerializeField]
    private Transform
        minimapEnableSpot,
        PolarityBarEnableSpot,
        healthBarEnableSpot,
        clockEnableSpot,
        NullBarEnableSpot,
        finalEnableSpot;


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

    void Start(){
        //curHeldItemSprite = GameObject.Find("ItemSprite").GetComponent<Image>();
        //curHeldItemSprite.enabled = false;

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


        if (!isDebug)
        {
            clock.SetActive(false);
            polarityBar.SetActive(false);
            healthBar.SetActive(false);
            minimap.SetActive(false);
            darkEnemies.SetActive(false);
            lightEnemies.SetActive(false);
            nullBar.SetActive(false);
        }


    }


    void Update()
    {


        if (CheckSharedGridPosition(player.transform, minimapEnableSpot))
        {
            if (minimap.activeSelf == false) 
                minimap.SetActive(true);
        }

        if (CheckSharedGridPosition(player.transform, PolarityBarEnableSpot))
        {
            if (polarityBar.activeSelf == false)
                polarityBar.SetActive(true);
        }


        if (GameObject.Find("TutRoom1Enemies").transform.childCount == 0 && GameObject.Find("PolarityTutDoor").GetComponent<DoorScript>().IsOpen == false)
        {
            GameObject.Find("PolarityTutDoor").GetComponent<DoorScript>().Open();
        }
    

        if (CheckSharedGridPosition(player.transform, healthBarEnableSpot))
        {
            if (healthBar.activeSelf == false)
                healthBar.SetActive(true);
        }

        if (CheckSharedGridPosition(player.transform, healthBarEnableSpot))
        {
            if (healthBar.activeSelf == false)
                healthBar.SetActive(true);
        }


        if (CheckSharedGridPosition(player.transform, clockEnableSpot))
        {
            if (clock.activeSelf == false)
                clock.SetActive(true);
        }

        if (CheckSharedGridPosition(player.transform, NullBarEnableSpot))
        {
            if (nullBar.activeSelf == false)
            {
                nullBar.SetActive(true);
            }

        }


        if ((CheckSharedGridPosition(player.transform, finalEnableSpot) && GameObject.Find("FinalTutDoor").GetComponent<DoorScript>().IsOpen == false))
        {
            GameObject.Find("FinalTutDoor").GetComponent<DoorScript>().Open();
            finalTutorialBlock.SetActive(true);
            darkEnemies.SetActive(true);
            lightEnemies.SetActive(true);
        }

        if (Input.GetKeyDown("r"))
        {
            SceneManager.LoadScene("Level1");
        }


        if (Input.GetKeyDown("o")) {
            DisplayDialogueWindow(dialogueWindow);
        } else if (Input.GetKeyDown("p")) {
            CloseDialogueWindow(dialogueWindow);
        }

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





    }

    public float GetDamageModifier()
    {
        switch(Mathf.Abs(polarity))
        {
            case 0:
                return 0;
            case 1:
                return .5f;
            case 2:
                return .75f;
            case 3:
                return 1.25f;
            case 4:
                return 2f;
            default:
                return 1f;
        }


    }

    public float GetDamageToEnemy()
    {
        switch (Mathf.Abs(polarity))
        {
            case 0:
                return 0;
            case 1:
                return 15f;
            case 2:
                return 20f;
            case 3:
                return 30f;
            case 4:
                return 60;
            default:
                return 1f;
        }


    }

    public float GetSlowdownFactor()
    {
        switch(Mathf.Abs(polarity))
        {
            case 0:
                return .66f;
            case 1:
                return .75f;
            case 2:
                return .9f;
            case 3:
                return 1f;
            case 4:
                return 1.1f;
            default:
                return 1f;
        }
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
        ticker.SetTickerOffset(offset);

        hand.RotateClockHand(0.20f * Mathf.Abs(offset));
    }

    public void TurnClock(float amount)
    {
        if (hand.isActiveAndEnabled)
            hand.RotateClockHand(amount);
    }

    public void DamagePlayer(int damage, string name)
    {
        playerHealth += damage;
        string desiredString = name + " " + (name == "Lightener" ? findGoodWords() : findBadWords()) + " Player for " + damage + " damage!";

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

        player.transform.Find("Main Camera").GetComponent<EffectShake>().DoShake(damage, true);



        if (playerHealth > playerMaxAbsoluteHealth || playerHealth < playerMaxAbsoluteHealth * -1)
        {
            playerHealth = Mathf.Clamp(playerHealth, playerMaxAbsoluteHealth * -1, playerMaxAbsoluteHealth);
            PlayerDeath();
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

    //

    public void PlayerDeath()
    {
        print("Threshhold broken! Game over!");
        GameObject.Find("Player").GetComponent<PlayerController>().enabled = false;
        GameObject.Find("Player").GetComponent<PlayerInput>().enabled = false;
        SceneManager.LoadScene("Level1");
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
