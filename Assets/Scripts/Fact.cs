using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Fact
{
    [Header("Basic Info")]
    public string factID;               // Unique identifier for the fact
    [TextArea]
    public string description;          // Description of the fact

    [Header("Statement Compatibility")]
    public List<string> compatibleStatementIDs = new List<string>(); // List of statement IDs
    public List<float> scoreMultipliers = new List<float>();         // Corresponding multipliers

    /// <summary>
    /// Returns the score multiplier associated with a given statement ID, or 1.0f if not found.
    /// </summary>
    public float GetMultiplierForStatement(string statementID)
    {
        for (int i = 0; i < compatibleStatementIDs.Count; i++)
        {
            if (compatibleStatementIDs[i] == statementID)
                return (i < scoreMultipliers.Count) ? scoreMultipliers[i] : 0.0f;
        }
        return 0.0f; // default multiplier
    }

    /// <summary>
    /// Adds or updates a compatible statement with its multiplier.
    /// </summary>
    public void AddOrUpdateCompatibility(string statementID, float multiplier)
    {
        int index = compatibleStatementIDs.IndexOf(statementID);
        if (index >= 0)
        {
            scoreMultipliers[index] = multiplier;
        }
        else
        {
            compatibleStatementIDs.Add(statementID);
            scoreMultipliers.Add(multiplier);
        }
    }

    /// <summary>
    /// Checks if this fact is compatible with a given statement.
    /// </summary>
    public bool IsCompatibleWith(string statementID)
    {
        return compatibleStatementIDs.Contains(statementID);
    }
}