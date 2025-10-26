using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI Elements")]
    public GameObject dialogueBox;     // parent panel for text + choices
    public TMP_Text dialogueText;
    public TMP_Text nameText;

    [Header("Choice UI")]
    public GameObject choiceButtonPrefab;    // Prefab for a choice button
    public Transform choiceButtonContainer;  // Parent for spawned buttons

    public float typingSpeed = 0.05f;

    private bool isTyping = false;
    private bool isDialogueActive = false;

    private Dictionary<string, DialogueLine> dialogueDict;
    private string currentLineID;

    public bool inChoice = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        dialogueBox.SetActive(false);
    }

    public void StartDialogue(string startLineID, Dictionary<string, DialogueLine> dialogueDictionary)
    {
        if (isDialogueActive) return;

        dialogueDict = dialogueDictionary;
        currentLineID = startLineID;
        isDialogueActive = true;
        dialogueBox.SetActive(true);

        ClearChoices();

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (currentLineID == null || !dialogueDict.ContainsKey(currentLineID))
        {
            EndDialogue();
            return;
        }

        DialogueLine line = dialogueDict[currentLineID];

        if (line.isChoiceLine)
        {
            inChoice = true;
            choiceButtonContainer.gameObject.SetActive(true);

            TalkManager.Instance.SetPlayerExpression("");
            TalkManager.Instance.SetNPCExpression("");

            if (line.speakerIsPlayer)
            {
                TalkManager.Instance.SetPlayerExpression("neutral");
                nameText = TalkManager.Instance.playerNameText;
            }
            else
            {
                TalkManager.Instance.SetNPCExpression("neutral");
                nameText = TalkManager.Instance.npcNameText;
            }

            dialogueText.text = "...";
            nameText.text = line.speakerName;

            foreach (DialogueChoice choice in line.choices)
            {
                GameObject btnObj = Instantiate(choiceButtonPrefab, choiceButtonContainer);
                ChoiceButton btn = btnObj.GetComponent<ChoiceButton>();
                btn.Init(choice.text, choice.nextLineID);
            }
        }
        else if (line.isAttackLine)
        {
            TalkManager.Instance.SetPlayerExpression("");
            TalkManager.Instance.SetNPCExpression("");

            TalkManager.Instance.SetPlayerExpression(line.expression);
            nameText = TalkManager.Instance.playerNameText;

            nameText.text = line.speakerName;

            dialogueText.text = line.text;

            AttackManager.Instance.StartEncounter(line.encounterID);
        }
        else
        {
            inChoice = false;


            if (line.hasFlag)
            {
                Debug.Log(line.flag);
                FlagManager.Instance.AddFlag(line.flag);
                FlagManager.Instance.CheckFlagActions(line.flag);
            }
            // Standard line
            StartCoroutine(TypeLine(line));
        }
    }

    private IEnumerator TypeLine(DialogueLine line)
    {
        isTyping = true;
        dialogueText.text = "";

        // Show only the speaking character
        TalkManager.Instance.SetPlayerExpression("");
        TalkManager.Instance.SetNPCExpression("");
        if (line.speakerIsPlayer)
        {
            TalkManager.Instance.SetPlayerExpression(line.expression);
            nameText = TalkManager.Instance.playerNameText;
        }
        else
        {
            TalkManager.Instance.SetNPCExpression(line.expression);
            nameText = TalkManager.Instance.npcNameText;
        }

        nameText.text = line.speakerName;

        foreach (char c in line.text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public void AdvanceDialogue()
    {
        if (!isDialogueActive) return;

        if (isTyping)
        {
            // finish current line instantly
            StopAllCoroutines();
            DialogueLine line = dialogueDict[currentLineID];
            dialogueText.text = line.text;
            isTyping = false;
        }
        else
        {
            DialogueLine line = dialogueDict[currentLineID];
            currentLineID = line.nextLineID;
            ShowCurrentLine();
        }
    }

    public void PreviewChoice(string previewLineID)
    {
        if (!dialogueDict.ContainsKey(previewLineID))
        {
            dialogueText.text = "*walks away*";
            return;
        }

        DialogueLine previewLine = dialogueDict[previewLineID];

        if (previewLine.speakerIsPlayer)
        {
            nameText = TalkManager.Instance.playerNameText;
        }
        else
        {
            nameText = TalkManager.Instance.npcNameText;
        }
        // Temporarily show dialogue text + portraits as if the preview line was active
        dialogueText.text = previewLine.text;
        nameText.text = previewLine.speakerName;

        TalkManager.Instance.SetPlayerExpression("");
        TalkManager.Instance.SetNPCExpression("");

        if (previewLine.speakerIsPlayer)
            TalkManager.Instance.SetPlayerExpression(previewLine.expression);
        else
            TalkManager.Instance.SetNPCExpression(previewLine.expression);
    }

    public void EndPreview()
    {
        // Restore the choice-line look
        if (!dialogueDict.ContainsKey(currentLineID)) return;
        ClearChoices();
        ShowCurrentLine();
    }

    // Called when a choice button is pressed
    public void SelectedChoice(string choiceNextID)
    {
        currentLineID = choiceNextID;

        ClearChoices();
        ShowCurrentLine();
    }

    public void AttackOver(bool success) //probably add diff dialogue for win or loss
    {
        if (success)
        {
            currentLineID = dialogueDict[currentLineID].nextLineID;
        }
        else
        {
            currentLineID = dialogueDict[currentLineID].failureLineID;
        }
        
        ShowCurrentLine();
    }

    private void EndDialogue()
    {
        dialogueBox.SetActive(false);
        TalkManager.Instance.SetPlayerExpression("");
        TalkManager.Instance.SetNPCExpression("");
        isDialogueActive = false;

        ClearChoices();

        // Unfreeze actors
        TalkManager.Instance.EndConversation();
    }

    private void ClearChoices()
    {
        foreach (Transform child in choiceButtonContainer)
        {
            Destroy(child.gameObject);
        }
        choiceButtonContainer.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space) && !inChoice)
        {
            AdvanceDialogue();
        }
    }
}