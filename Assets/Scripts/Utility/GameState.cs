using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameState : MonoBehaviour
{
    public ClockHand hand;
    public PolarityBarTicker ticker;
    public int playerHealth = 0;

    public int playerMaxAbsoluteHealth = 100;
    public int polarity;

    public Vector3 clockRotation = new Vector3(0f, 0f, 0f);

    public GameObject ActionTextPrefab;

    public GameObject dialogueWindow;

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
    }

    private void FixedUpdate()  
    {

    }
    void Update()
    {
        if (Input.GetKeyDown("o")) {
            DisplayDialogueWindow(dialogueWindow);
        } else if (Input.GetKeyDown("p")) {
            CloseDialogueWindow(dialogueWindow);
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

    public float GetSlowdownFactor()
    {
        switch(Mathf.Abs(polarity))
        {
            case 0:
                return 1f;
            case 1:
                return 1f;
            case 2:
                return .9f;
            case 3:
                return .75f;
            case 4:
                return .5f;
            default:
                return 1f;
        }
    }
    
    public void ChangePolarity(int offset)
    {
        ticker.SetTickerOffset(offset);
        // TODO: Look into this
        hand.RotateClockHand(0.20f * Mathf.Abs(offset));
    }

    public void TurnClock(float amount)
    {
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





        if (playerHealth > playerMaxAbsoluteHealth || playerHealth < playerMaxAbsoluteHealth * -1)
        {
            playerHealth = Mathf.Clamp(playerHealth, playerMaxAbsoluteHealth * -1, playerMaxAbsoluteHealth);
            print("Threshhold broken! Game over!");
            GameObject.Find("Player").GetComponent<PlayerController>().enabled = false;
        }

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

}
