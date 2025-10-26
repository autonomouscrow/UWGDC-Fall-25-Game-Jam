using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public enum Meters
{
    Sus,
    Mood,
    Confused,
    Aggro,
}

public class AttackManager : MonoBehaviour
{
    public static AttackManager Instance;
    public GameObject atkDefPrefab;
    public GameObject TRUTHNotePrefab;

    public bool attackOn = false;

    public enum AttackMode
    {
        ChooseAction,
        AttackDef,
        Special
    }

    private AttackMode currentMode = AttackMode.ChooseAction;

    [Header("UI Panels")]
    public GameObject attackParent;

    public Image playerAttackPortrait;
    public Image npcAttackPortrait;
    public GameObject chooseActionUI;  // Default "Attack / Special / Item" menu
    public GameObject attackDefUI;     // Attack/Defense mode UI
    public Transform atkDefsContainer;
    public Transform notePad;
    public GameObject specialUI;       // Special attack selection UI

    public Image DetectOVisionPoints;
    public Image DetectOVisionTruths;
    

    public string atkOrDef = "";


    [Header("Encounter Settings")]
    public float SusMeter = 0f;               // Current suspicion level
    public float MoodMeter = 50f;              // large is happy, small is sad
    public float ConfusedMeter = 0f;         //
    public float AggroMeter = 0f;

    public float ShowEmotionLimit = 50f;
    public float CubismLimit = 75f;

    public float CubismTax = 10f;

    public bool LastTurnCubism = false;

    public List<KeyValuePair<float, string>> emotionValues = new List<KeyValuePair<float, string>>();

    public float TRUTH = 0f;
    public int maxTRUTHNote = -1;
    public bool doneTruths = false;
    public string currNPCEmotion = "neutral";
    private bool playerChoseAction = false;   // Whether the player has acted this turn
    private string currentEncounterID = "";
    private string pendingAttackID = "";      // Stores the ID of the selected attack

    public FightData fightData;

    public List<string> statementSatchel = new List<string>();

    public bool inAttackLoop = false;

    public bool inSpecial = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        AggroMeter = 5;
        CheckEmotions();

        attackOn = false;
        UpdateUI();
    }

    /// <summary>
    /// Called by DialogueManager to begin an encounter.
    /// </summary>
    public void StartEncounter(string encounterID)
    {
        currentEncounterID = encounterID;
        pendingAttackID = "";
        playerChoseAction = false;
        attackOn = true;

        DetectOVisionTruths.fillAmount = 0;

        Debug.Log($"[AttackManager] Encounter '{encounterID}' started.");

        fightData=FightsDatabase.GetFightData(currentEncounterID);

        SpecialAttackManager.Instance.existingArguments = new HashSet<string>();

        SusMeter = 0f;
        MoodMeter = 50f;
        ConfusedMeter = 0f;
        AggroMeter = 0f;

        LastTurnCubism = false;

        TRUTH = 0f;
        maxTRUTHNote = -1;
        doneTruths = false;
        currNPCEmotion = "neutral";

        inAttackLoop = false;

        inSpecial = false;

        statementSatchel = new List<string>();

        foreach (Transform child in notePad)
        {
            Destroy(child.gameObject);
        }

        StartAttack();
    }

    /// <summary>
    /// Enables the attack UI and begins the attack loop.
    /// </summary>
    public void StartAttack()
    {
        if (attackParent != null)
            attackParent.SetActive(true);

        Debug.Log("[AttackManager] Attack UI enabled. Starting Attack Loop...");
        inAttackLoop = true;
        StartCoroutine(AttackLoop());
    }

    /// <summary>
    /// Main turn-based combat loop.
    /// </summary>
    private IEnumerator AttackLoop()
    {

        SetPlayerAttackExpression("atk");

        CheckEmotions();

        GatherTruths();
        currentMode = AttackMode.ChooseAction;
        UpdateUI();

        while (inAttackLoop)
        {
            Debug.Log($"[AttackManager] --- Turn Start --- (SusMeter: {SusMeter})");
            playerChoseAction = false;
            pendingAttackID = "";

            Debug.Log("[AttackManager] Waiting for player attack selection...");
            yield return new WaitUntil(() => playerChoseAction);

            Debug.Log($"[AttackManager] Player used attack: {pendingAttackID}");
            yield return ResolveAttack(pendingAttackID);

            if (!inSpecial)
            {
                CheckEmotions();

                GatherTruths();

                currentMode = AttackMode.ChooseAction;
                UpdateUI();
            }

            if (CheckEncounterEnd())
            {
                EndAttack(false);
                yield break;
            }

            Debug.Log("[AttackManager] --- Turn End ---");
            yield return null;
        }
    }

    /// <summary>
    /// Called by UI buttons to trigger an attack.
    /// </summary>
    public void UseAttack(string attackID)
    {
        if (!inAttackLoop) return;
        if (playerChoseAction) return; // prevent double input in one turn

        pendingAttackID = attackID;
        playerChoseAction = true;
    }

    /// <summary>
    /// Simulate the effects of an attack.
    /// Replace this with your real attack logic later.
    /// </summary>
    private IEnumerator ResolveAttack(string attackID)
    {
        

        AtkDef ad = PlayerInvManager.Instance.atkdefs[attackID];

        if (ad.type == AtkDefType.standard)
        {
            SetPlayerAttackExpression("atk");
        }
        else if (ad.type == AtkDefType.cheerful)
        {
            SetPlayerAttackExpression("happy");
        }
        else if (ad.type == AtkDefType.confused)
        {
            SetPlayerAttackExpression("confused");
        }
        else if (ad.type == AtkDefType.aggressive)
        {
            SetPlayerAttackExpression("angry");
        }
        else if (ad.type == AtkDefType.sad)
        {
            SetPlayerAttackExpression("sad");
        }
        else if (ad.type == AtkDefType.sexy)
        {
            SetPlayerAttackExpression("sexy");
        }
        else
        {
            SetPlayerAttackExpression("cubism");
        }
        

            Debug.Log($"[AttackManager] Resolving attack {attackID}...");
        yield return new WaitForSeconds(0.5f);

        //truth; sus; happy; sad; confused; angry; sexy
        TRUTH += ad.vals[0];
        SusMeter += ad.vals[1] * fightData.emotionMultipliers[0];
        MoodMeter += ad.vals[2] * fightData.emotionMultipliers[1];
        MoodMeter -= ad.vals[3] * fightData.emotionMultipliers[2];
        ConfusedMeter += ad.vals[4] * fightData.emotionMultipliers[3];
        AggroMeter += ad.vals[5] * fightData.emotionMultipliers[4];

        if (fightData.lustEffect == Meters.Sus)
        {
            SusMeter += ad.vals[6] * fightData.lustMultiplier;
        }
        else if (fightData.lustEffect == Meters.Mood)
        {
            MoodMeter += ad.vals[6] * fightData.lustMultiplier;
        }
        else if (fightData.lustEffect == Meters.Confused)
        {
            ConfusedMeter += ad.vals[6] * fightData.lustMultiplier;
        }
        else
        {
            AggroMeter += ad.vals[6] * fightData.lustMultiplier;
        }

        SusMeter = Mathf.Clamp(SusMeter, 0, 100);
        MoodMeter = Mathf.Clamp(MoodMeter, 0, 100);
        ConfusedMeter = Mathf.Clamp(ConfusedMeter, 0, 100);
        AggroMeter = Mathf.Clamp(AggroMeter, 0, 100);
    }

    private void CheckEmotions()
    {

        emotionValues.Add(new KeyValuePair<float, string>(SusMeter, "sus"));
        if (MoodMeter > 50)
        {
            emotionValues.Add(new KeyValuePair<float, string>((MoodMeter - 50) * 2, "happy"));
        }
        else
        {
            emotionValues.Add(new KeyValuePair<float, string>((50 - MoodMeter) * 2, "sad"));
        }
        emotionValues.Add(new KeyValuePair<float, string>(ConfusedMeter, "confused"));
        emotionValues.Add(new KeyValuePair<float, string>(AggroMeter, "angry"));


        emotionValues.Sort((a, b) => (a.Key > b.Key) ? -1 : ((a.Key == b.Key) ? 0 : 1));

        KeyValuePair<float, string> topEmotion = emotionValues[0];

        if (topEmotion.Value == "sus" && topEmotion.Key == 100)
        {
            EndAttack(false);
        }

        if (topEmotion.Key >= CubismLimit)
        {
            if (!LastTurnCubism)
            {
                SusMeter += CubismTax / 2;
            }
            else
            {
                SusMeter += CubismTax;
            }
            LastTurnCubism = true;
            SetNPCAttackExpression("cubism");
            currNPCEmotion = "cubism";
        }
        else if (topEmotion.Key >= ShowEmotionLimit)
        {
            SetNPCAttackExpression(topEmotion.Value);
            currNPCEmotion = topEmotion.Value;
        }
        else
        {
            SetNPCAttackExpression("neutral");
            currNPCEmotion = "neutral";
        }
        emotionValues = new List<KeyValuePair<float, string>>();
    }

    private void GatherTruths()
    {
        bool keepGathering = false;
        Debug.Log(maxTRUTHNote * 100);
        if ((maxTRUTHNote * 100 >= TRUTH) || doneTruths)
        {
            return;
        }

        for (int i = maxTRUTHNote + 1; i <= TRUTH / 100; i++)
        {
            Debug.Log("textaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" + i);
            if (i >= fightData.notes.Count)
            {
                doneTruths = true;
                break;
            }
            Note currNote = fightData.notes[i];

            if (currNote.noteReq == "none" || currNote.noteReq == currNPCEmotion)
            {
                if (currNote.noteType == NoteType.text)
                {

                    GameObject noteObj = Instantiate(TRUTHNotePrefab, notePad);
                    noteObj.GetComponentInChildren<TMP_Text>().text = $"{currNote.noteText}";
                }
                else if (currNote.noteType == NoteType.statement)
                {
                    GameObject noteObj = Instantiate(TRUTHNotePrefab, notePad);
                    noteObj.GetComponentInChildren<TMP_Text>().text = $"<u>{currNote.noteText}</u>";
                    statementSatchel.Add(currNote.statementID);
                }
                //todo the other options
                if (currNote.setTruth)
                {
                    TRUTH = currNote.TRUTHSET;
                    keepGathering = true;
                }

                maxTRUTHNote = i;
            }
        }
        if (keepGathering)
        {
            GatherTruths();
        }
    }

    public void ActivateSpecial()
    {
        inSpecial = true;
        SetMode("special");
        SpecialAttackManager.Instance.allStatementIDs = statementSatchel;
        SpecialAttackManager.Instance.OpenSpecialAttackMenu();

    }

    public void NvmSpecial()
    {
        inSpecial = false;
        SpecialAttackManager.Instance.CloseSpecialAttackMenu();
        SetMode("back");
    }

    public void FailedSpecial()
    {
        inSpecial = false;
        SpecialAttackManager.Instance.CloseSpecialAttackMenu();
        SetMode("back");

        SusMeter += 50; //punishment

        SetPlayerAttackExpression("cubejection");
        CheckEmotions();
    }

    /// <summary>
    /// Determines if the current encounter should end (manual exit).
    /// </summary>
    private bool CheckEncounterEnd()
    {
        // Example: Press Escape to end encounter
        return false;
    }

    /// <summary>
    /// Ends the current attack and signals the DialogueManager to resume dialogue.
    /// </summary>
    public void EndAttack(bool success)
    {
        if (success)
        {
            SetPlayerAttackExpression("objection");
        }

        StartCoroutine(EndAtk(success));
    }

    private IEnumerator EndAtk(bool success)
    {
        if (success)
        {
            chooseActionUI.SetActive(false);
            attackDefUI.SetActive(false);
            specialUI.SetActive(false);
            yield return new WaitForSeconds(2f);
        }

        attackOn = false;
        currentMode = AttackMode.ChooseAction;
        UpdateUI();

        Debug.Log("encounter success?" + success);

        inAttackLoop = false;

        DialogueManager.Instance.AttackOver(success);

        yield return null;
    }

    // Called by UI buttons
    public void SetMode(string modeName)
    {
        Debug.Log(modeName);
        switch (modeName.ToLower())
        {
            case "attack":
                currentMode = AttackMode.AttackDef;
                atkOrDef = "attack";
                break;
            case "def":
                currentMode = AttackMode.AttackDef;
                atkOrDef = "def";
                break;
            case "special":
                currentMode = AttackMode.Special;
                atkOrDef = "";
                break;
            case "back":
                currentMode = AttackMode.ChooseAction;
                atkOrDef = "";
                break;
            default:
                Debug.LogWarning($"Unknown mode: {modeName}");
                atkOrDef = "";
                return;
        }

        UpdateUI();
    }

    public void SetPlayerAttackExpression(string exprName)
    {
        if (TalkManager.Instance.playerTalkSprites == null || string.IsNullOrEmpty(exprName))
        {
            playerAttackPortrait.sprite = null;
            playerAttackPortrait.enabled = false;
            return;
        }

        Sprite s = TalkManager.Instance.playerTalkSprites.GetSprite(exprName);
        playerAttackPortrait.sprite = s;
        playerAttackPortrait.enabled = s != null;

        playerAttackPortrait.preserveAspect = true;
    }

    public void SetNPCAttackExpression(string exprName)
    {
        if (TalkManager.Instance.npcTalkSprites == null || string.IsNullOrEmpty(exprName))
        {
            npcAttackPortrait.sprite = null;
            npcAttackPortrait.enabled = false;
            return;
        }

        Sprite s = TalkManager.Instance.npcTalkSprites.GetSprite(exprName);
        npcAttackPortrait.sprite = s;
        npcAttackPortrait.enabled = s != null;

        npcAttackPortrait.preserveAspect = true;
    }

    private void UpdateUI()
    {
        attackParent.SetActive(attackOn);
        // Toggle panels depending on mode
        chooseActionUI.SetActive(currentMode == AttackMode.ChooseAction);
        attackDefUI.SetActive(currentMode == AttackMode.AttackDef);
        specialUI.SetActive(currentMode == AttackMode.Special);

        if (currentEncounterID == "TUTORIALDUCK")
        {
            DetectOVisionPoints.fillAmount = 0.8f*Mathf.Clamp(TRUTH / ((fightData.notes.Count - 11) * 100),0,1);
            DetectOVisionTruths.fillAmount = 0.8f*Mathf.Clamp((maxTRUTHNote >= 0 ? ((float)maxTRUTHNote) : 0) / (fightData.notes.Count - 11),0,1);
        }
        else
        {  
            DetectOVisionPoints.fillAmount = TRUTH / ((fightData.notes.Count-1) * 100);
            DetectOVisionTruths.fillAmount = (maxTRUTHNote>=0?((float)maxTRUTHNote):0) / (fightData.notes.Count-1);
        }

        if (currentMode == AttackMode.AttackDef)
        {
            foreach (Transform child in atkDefsContainer)
            {
                Destroy(child.gameObject);
            }

            if (atkOrDef == "attack")
            {
                foreach (AtkDef atk in PlayerInvManager.Instance.atks)
                {
                    GameObject atkDefObj = Instantiate(atkDefPrefab, atkDefsContainer);
                    atkDefObj.GetComponentInChildren<TMP_Text>().text = $"{atk.name} - {atk.flavTxt}";
                    atkDefObj.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        UseAttack(atk.id);
                    });
                }
            }
            else if (atkOrDef == "def")
            {
                foreach (AtkDef atk in PlayerInvManager.Instance.defs)
                {
                    GameObject atkDefObj = Instantiate(atkDefPrefab, atkDefsContainer);
                    atkDefObj.GetComponentInChildren<TMP_Text>().text = $"{atk.name} - {atk.flavTxt}";
                    atkDefObj.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        UseAttack(atk.id);
                    });
                }
            }
        }
    }

    public void ResetToDefault()
    {
        currentMode = AttackMode.ChooseAction;
        UpdateUI();
    }

    public AttackMode GetCurrentMode() => currentMode;
}