using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerInvManager : MonoBehaviour
{
    public static PlayerInvManager Instance;

    [Header("Inventory Lists")]
    public List<AtkDef> atks = new List<AtkDef>();
    public List<AtkDef> defs = new List<AtkDef>();
    public List<string> fax = new List<string>();

    public Dictionary<string, AtkDef> atkdefs = new Dictionary<string, AtkDef>();
    public Dictionary<string, Fact> allFacts = new Dictionary<string, Fact>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        LoadAtkDefsFromFile();
        LoadFactsFromFile();

        AddAttack("stdA1");
        AddAttack("stdA2");

        AddDefense("stdD1");
        AddDefense("stdD2");

        AddFact("BROSTABBED");
        AddFact("ALLERGY");

        AddAttack("DEBUG");
    }

    /// <summary>
    /// Reads a text file and creates a dictionary of AtkDef objects.
    /// Format: ID; name; flavTxt; type
    /// </summary>
    public void LoadAtkDefsFromFile()
    {
        atkdefs = new Dictionary<string, AtkDef>();
        string fileName = "AtkDefs";

        TextAsset file = Resources.Load(fileName) as TextAsset;

        if (fileName==null)
        {
            Debug.LogError($"[PlayerInvManager] File not found: {fileName}");
        }

        string[] lines = file.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);


        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(';');
            if (parts.Length < 4)
            {
                Debug.LogWarning($"[PlayerInvManager] Invalid line format: {line}");
                continue;
            }

            string id = parts[0].Trim();
            string name = parts[1].Trim();
            string flavTxt = parts[2].Trim();
            string typeStr = parts[3].Trim();

            if (!System.Enum.TryParse(typeStr, true, out AtkDefType type))
            {
                Debug.LogError($"[PlayerInvManager] Unknown AtkDefType '{typeStr}' in line: {line}");
            }

            List<float> vals = new List<float>();
            vals.Add(float.Parse(parts[4].Trim()));
            vals.Add(float.Parse(parts[5].Trim()));
            vals.Add(float.Parse(parts[6].Trim()));
            vals.Add(float.Parse(parts[7].Trim()));
            vals.Add(float.Parse(parts[8].Trim()));
            vals.Add(float.Parse(parts[9].Trim()));
            vals.Add(float.Parse(parts[10].Trim()));

            AtkDef atkdef = new AtkDef(id, name, flavTxt, type, vals);
            atkdefs[id] = atkdef;
        }

        Debug.Log($"[PlayerInvManager] Loaded {atkdefs.Count} AtkDefs from file.");
    }

    public void LoadFactsFromFile()
    {
        allFacts = new Dictionary<string, Fact>();
        string fileName = "Facts";

        TextAsset file = Resources.Load(fileName) as TextAsset;

        if (fileName==null)
        {
            Debug.LogError($"[PlayerInvManager] File not found: {fileName}");
        }

        string[] lines = file.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);


        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(';');
            if (parts.Length < 4)
            {
                Debug.LogWarning($"[PlayerInvManager] Invalid line format: {line}");
                continue;
            }

            

            Fact fact = new Fact();

            string id = parts[0].Trim();
            string desc = parts[1].Trim();
            string stmtIDs = parts[2].Trim();
            string scores = parts[3].Trim();

            if (stmtIDs.StartsWith("~") && stmtIDs.EndsWith("~"))
                    stmtIDs = stmtIDs.Substring(1, stmtIDs.Length - 2);
            if (scores.StartsWith("~") && scores.EndsWith("~"))
                    scores = scores.Substring(1, scores.Length - 2);
            
            fact.factID = id;
            fact.description = desc;
            fact.compatibleStatementIDs = stmtIDs.Split("||").Select(s => s.Trim()).ToList();
            fact.scoreMultipliers = scores.Split("||").Select(float.Parse).ToList();

            allFacts[id] = fact;
        }

        Debug.Log($"[PlayerInvManager] Loaded {allFacts.Count} Facts from file.");
    }

    public void AddAttack(string id)
    {
        if (id == null || atks.Exists(a => a.id == id)) return;
        atks.Add(atkdefs[id]);
        Debug.Log($"[PlayerInvManager] Added Attack: {id} ({atkdefs[id].type})");
    }

    public void AddDefense(string id)
    {
        if (id == null || defs.Exists(a => a.id == id)) return;
        defs.Add(atkdefs[id]);
        Debug.Log($"[PlayerInvManager] Added Defense: {id} ({atkdefs[id].type})");
    }

    public void RemoveAtkDef(string id)
    {
        atks.RemoveAll(a => a.id == id);
        defs.RemoveAll(d => d.id == id);
    }

    public void AddFact(string factID)
    {
        if (!fax.Contains(factID))
        {
            fax.Add(factID);
            Debug.Log($"[PlayerInvManager] Added Fact: {factID}");
        }
    }

    public bool HasFact(string factID) => fax.Contains(factID);

    /// <summary>
    /// Clears all inventory categories.
    /// </summary>
    public void ClearAll()
    {
        atks.Clear();
        defs.Clear();
        fax.Clear();
        Debug.Log("[PlayerInvManager] Inventory cleared.");
    }
    
    
}
