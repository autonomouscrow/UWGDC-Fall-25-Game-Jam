using UnityEngine;
using System.Collections.Generic;
public class AllStatements : MonoBehaviour
{
    public static AllStatements Instance;
    public Dictionary<string, string> allStatements = new Dictionary<string, string>();

    // Update is called once per frame
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        LoadStatementsFromFile();
    }
    
    public void LoadStatementsFromFile()
    {
        allStatements = new Dictionary<string, string>();
        string fileName = "Statements";

        TextAsset file = Resources.Load(fileName) as TextAsset;

        if (fileName==null)
        {
            Debug.LogError($"[AllStatements] File not found: {fileName}");
        }

        string[] lines = file.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);


        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(';');
            if (parts.Length < 2)
            {
                Debug.LogWarning($"[AllStatements] Invalid line format: {line}");
                continue;
            }

            

            string id = parts[0].Trim();
            string desc = parts[1].Trim();

            allStatements[id] = desc;
        }

        Debug.Log($"[AllStatements] Loaded {allStatements.Count} Statements from file.");
    }
}
