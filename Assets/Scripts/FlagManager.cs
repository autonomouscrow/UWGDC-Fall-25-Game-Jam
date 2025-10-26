using System.Collections.Generic;
using UnityEngine;

public class FlagManager : MonoBehaviour
{
    public static FlagManager Instance;

    // All current game flags
    private HashSet<string> activeFlags = new HashSet<string>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Adds a flag if it doesn't already exist.
    /// </summary>
    public void AddFlag(string flag)
    {
        if (string.IsNullOrEmpty(flag)) return;

        if (!activeFlags.Contains(flag))
        {
            activeFlags.Add(flag);
            Debug.Log($"[FlagManager] Added flag: {flag}");
        }
    }

    /// <summary>
    /// Removes a flag from the active set.
    /// </summary>
    public void RemoveFlag(string flag)
    {
        if (activeFlags.Remove(flag))
        {
            Debug.Log($"[FlagManager] Removed flag: {flag}");
        }
    }

    /// <summary>
    /// Checks if a specific flag is active.
    /// </summary>
    public bool HasFlag(string flag)
    {
        if (flag == "")
        {
            return true;
        }
        return activeFlags.Contains(flag);
    }

    /// <summary>
    /// Returns a list of all active flags (read-only copy).
    /// </summary>
    public List<string> GetAllFlags()
    {
        return new List<string>(activeFlags);
    }

    /// <summary>
    /// Clears all flags (use with caution).
    /// </summary>
    public void ClearAllFlags()
    {
        activeFlags.Clear();
        Debug.Log("[FlagManager] All flags cleared.");
    }

    public void CheckFlagActions(string flag)
    {
        if (flag == "TALKEDTOBH")
        {
            PlayerInvManager.Instance.AddFact("LIKESWAMEN");
        }
        if (flag == "TALKEDTOWH")
        {
            PlayerInvManager.Instance.AddFact("SELLSGUNS");
            PlayerInvManager.Instance.AddFact("QUIET");
        }
        if (flag == "TALKEDTOPH")
        {
            PlayerInvManager.Instance.AddFact("KNIFE");
        }
        if (flag == "ENDING1")
        {
            EndingManager.Instance.SetEnding("stab");
        }
    }
}
