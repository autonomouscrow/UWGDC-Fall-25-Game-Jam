using UnityEngine;
using System.Collections.Generic;

public class NPCInteraction : MonoBehaviour
{
    [Header("UI Prompt")]
    public GameObject interactUI; // "Press T to Talk" popup

    public List<SPair> flagDialog = new List<SPair>();

    private bool playerNearby = false;
    private GameObject player;
    private TalkSprites npcTalkSprites;

    void Start()
    {
        npcTalkSprites = GetComponentInParent<TalkSprites>();

        if (interactUI != null)
            interactUI.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            player = other.gameObject;

            if (interactUI != null)
                interactUI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            player = null;

            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }

    void Update()
    {
        if (playerNearby && InputManager.Talk)
        {
            // Hide popup when talking begins
            if (interactUI != null)
                interactUI.SetActive(false);

            string newestDialog = "";

            foreach (SPair fd in flagDialog)
            {
                if (FlagManager.Instance.HasFlag(fd.k))
                {
                    newestDialog = fd.v;
                }
            }
            TalkManager.Instance.StartConversation(
                player.GetComponent<TalkSprites>(),
                npcTalkSprites,
                "1",
                DialogueLoader.LoadDialogue(newestDialog)
            );
        }
    }
}
