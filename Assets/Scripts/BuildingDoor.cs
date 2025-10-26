using UnityEngine;
using UnityEngine.UI;

public class BuildingDoor : MonoBehaviour
{
    [Header("Settings")]
    public Transform insidePosition;    // Where to teleport the player inside
    public GameObject interactUI;       // UI prompt (e.g. "Press E to Enter")

    private bool isPlayerNearby = false;
    private GameObject player;

    void Start()
    {
        if (interactUI != null)
            interactUI.SetActive(false); // Hide at start
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(Keybinds.Interact))
        {
            EnterBuilding();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            player = other.gameObject;

            if (interactUI != null)
                interactUI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            player = null;

            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }

    void EnterBuilding()
    {
        if (player != null && insidePosition != null)
        {
            player.transform.position = insidePosition.position;
        }

        if (interactUI != null)
            interactUI.SetActive(false);

        isPlayerNearby = false;
    }
}
