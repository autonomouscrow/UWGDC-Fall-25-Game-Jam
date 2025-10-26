using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TalkManager : MonoBehaviour
{
    public static TalkManager Instance;

    [Header("UI Elements")]
    public Image playerPortrait;
    public Image npcPortrait;

    public GameObject playerNameTag;
    public GameObject npcNameTag;

    public TMP_Text playerNameText;
    public TMP_Text npcNameText;

    public TalkSprites playerTalkSprites;
    public TalkSprites npcTalkSprites;
    private bool inTalkMode = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        inTalkMode = true;
        EndConversation();
    }

    /// <summary>
    /// Begins a conversation between player and NPC starting at a given line ID.
    /// </summary>
    /// <param name="player">Player TalkSprites component</param>
    /// <param name="npc">NPC TalkSprites component</param>
    /// <param name="startLineID">Dialogue start line ID</param>
    /// <param name="dialogueDict">Dictionary of dialogue lines</param>
    public void StartConversation(TalkSprites player, TalkSprites npc, string startLineID, Dictionary<string, DialogueLine> dialogueDict)
    {
        if (inTalkMode) return;

        inTalkMode = true;
        playerTalkSprites = player;
        npcTalkSprites = npc;

        // Freeze actors
        SetActorsFrozen(true);

        // Hide both portraits initially
        SetPlayerExpression("");
        SetNPCExpression("");

        // Begin dialogue
        DialogueManager.Instance.StartDialogue(startLineID, dialogueDict);
    }

    public void EndConversation()
    {
        if (!inTalkMode) return;

        inTalkMode = false;

        SetActorsFrozen(false);
        SetPlayerExpression("");
        SetNPCExpression("");
    }

    public void SetPlayerExpression(string exprName)
    {
        if (playerTalkSprites == null || string.IsNullOrEmpty(exprName))
        {
            playerPortrait.sprite = null;
            playerPortrait.enabled = false;
            if (playerNameTag != null) playerNameTag.SetActive(false);
            return;
        }

        Sprite s = playerTalkSprites.GetSprite(exprName);
        playerPortrait.sprite = s;
        playerPortrait.enabled = s != null;
        if (playerNameTag != null) playerNameTag.SetActive(s != null);

        playerPortrait.preserveAspect = true;
    }

    public void SetNPCExpression(string exprName)
    {
        if (npcTalkSprites == null || string.IsNullOrEmpty(exprName))
        {
            npcPortrait.sprite = null;
            npcPortrait.enabled = false;
            if (npcNameTag != null) npcNameTag.SetActive(false);
            return;
        }

        Sprite s = npcTalkSprites.GetSprite(exprName);
        npcPortrait.sprite = s;
        npcPortrait.enabled = s != null;
        if (npcNameTag != null) npcNameTag.SetActive(s != null);

        npcPortrait.preserveAspect = true;
    }

    public void SetActorsFrozen(bool frozen)
    {
        foreach (var mover in FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None))
            mover.enabled = !frozen;
    }
}