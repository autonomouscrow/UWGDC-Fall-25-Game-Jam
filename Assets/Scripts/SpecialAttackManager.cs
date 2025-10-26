using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpecialAttackManager : MonoBehaviour
{
    public static SpecialAttackManager Instance;

    [Header("UI References")]
    public GameObject specialAttackParent;   // Main UI container (hidden when inactive)
    public Transform factListParent;         // Parent for fact buttons
    public Transform statementListParent;    // Parent for statement buttons
    public Transform argumentListParent;     // Parent for created argument blocks

    public GameObject factButtonPrefab;
    public GameObject statementButtonPrefab;
    public GameObject argumentBlockPrefab;

    [Header("Data Sources")]
    public PlayerInvManager playerInv;       // Holds known facts
    public List<string> allStatementIDs;     // List of all statement IDs

    private string selectedFactID = null;
    private string selectedStatementID = null;

    public HashSet<string> existingArguments = new HashSet<string>(); // to prevent duplicates

    public float winThreshold = 100;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        specialAttackParent.SetActive(false);
    }

    public void OpenSpecialAttackMenu()
    {
        specialAttackParent.SetActive(true);
        RefreshFactList();
        RefreshStatementList();
        RefreshArgumentList();
    }

    public void CloseSpecialAttackMenu()
    {
        specialAttackParent.SetActive(false);
        selectedFactID = null;
        selectedStatementID = null;
    }

    private void RefreshFactList()
    {
        foreach (Transform child in factListParent)
            Destroy(child.gameObject);

        foreach (string factID in playerInv.fax)
        {
            GameObject btnObj = Instantiate(factButtonPrefab, factListParent);
            TMP_Text text = btnObj.GetComponentInChildren<TMP_Text>();
            text.text = playerInv.allFacts[factID].description;

            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() => OnFactSelected(factID));
        }
    }

    private void RefreshStatementList()
    {
        foreach (Transform child in statementListParent)
            Destroy(child.gameObject);

        foreach (string statementID in allStatementIDs)
        {
            GameObject btnObj = Instantiate(statementButtonPrefab, statementListParent);
            TMP_Text text = btnObj.GetComponentInChildren<TMP_Text>();
            text.text = AllStatements.Instance.allStatements[statementID];

            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() => OnStatementSelected(statementID));
        }
    }

    private void RefreshArgumentList()
    {
        foreach (Transform child in argumentListParent)
            Destroy(child.gameObject);

        foreach (string argKey in existingArguments)
        {
            string[] parts = argKey.Split('|');
            string factID = parts[0];
            string statementID = parts[1];

            GameObject argObj = Instantiate(argumentBlockPrefab, argumentListParent);
            Argument arg = argObj.GetComponent<Argument>();
            arg.factText.text = playerInv.allFacts[factID].description;
            arg.noteText.text = AllStatements.Instance.allStatements[statementID];

            Button deleteButton = argObj.transform.Find("Del").GetComponent<Button>();
            deleteButton.onClick.AddListener(() => RemoveArgument(factID, statementID));
        }
    }

    private void OnFactSelected(string factID)
    {
        selectedFactID = factID;
        TryCreateArgument();
    }

    private void OnStatementSelected(string statementID)
    {
        selectedStatementID = statementID;
        TryCreateArgument();
    }

    private void TryCreateArgument()
    {
        if (string.IsNullOrEmpty(selectedFactID) || string.IsNullOrEmpty(selectedStatementID) || existingArguments.Count == 4)
            return;

        string key = $"{selectedFactID}|{selectedStatementID}";

        if (existingArguments.Contains(key))
        {
            Debug.Log($"[SpecialAttackManager] Duplicate argument ignored: {key}");
            selectedFactID = null;
            selectedStatementID = null;
            return;
        }

        existingArguments.Add(key);
        RefreshArgumentList();

        Debug.Log($"[SpecialAttackManager] Created Argument: {selectedFactID} + {selectedStatementID}");

        // reset selection after argument creation
        selectedFactID = null;
        selectedStatementID = null;
    }

    private void RemoveArgument(string factID, string statementID)
    {
        string key = $"{factID}|{statementID}";
        if (existingArguments.Remove(key))
        {
            Debug.Log($"[SpecialAttackManager] Removed Argument: {key}");
            RefreshArgumentList();
        }
    }

    public void CheckArgumentScore()
    {
        float score = 0f;
        foreach (string argKey in existingArguments)
        {
            string[] parts = argKey.Split('|');
            string factID = parts[0];
            string statementID = parts[1];

            Fact fact = playerInv.allFacts[factID];

            score += fact.GetMultiplierForStatement(statementID);

            if (factID == "BROSTABBED" && statementID == "DUCKSTAB")
            {
                FlagManager.Instance.AddFlag("DRUCKTHEKILLER");
            }
        }

        if (score >= winThreshold)
        {
            AttackManager.Instance.EndAttack(true);
        }
        else
        {
            AttackManager.Instance.FailedSpecial();
        }
    }
}
