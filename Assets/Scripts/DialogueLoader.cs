using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class DialogueLoader
{
    public static Dictionary<string, DialogueLine> LoadDialogue(string fileName)
    {
        Dictionary<string, DialogueLine> dialogueDict = new Dictionary<string, DialogueLine>();
        TextAsset file = Resources.Load(fileName) as TextAsset;

        if (file == null)
        {
            Debug.LogError("Dialogue file not found: " + fileName);
            return dialogueDict;
        }

        string[] lines = file.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);


        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();
            if (line.StartsWith("#") || string.IsNullOrEmpty(line)) continue;

            // Split the first three fields: lineID, type, speakerName
            string[] firstParts = line.Split(new char[] { ';' }, 5);
            if (firstParts.Length < 3)
            {
                Debug.LogWarning("Invalid line header: " + line);
                continue;
            }

            string lineID = firstParts[0].Trim();
            string type = firstParts[1].Trim().ToLower();
            string speakerName = firstParts[2].Trim();

            if (type == "standard")
            {
                // Standard line: lineID, standard, speakerName, isPlayer, expression, text, nextLineID
                string[] parts = line.Split(new char[] { ';' }, 8);

                Debug.Log(string.Join(",", firstParts));
                Debug.Log($"string.Join(\",\", parts) + {parts.Length}");

                if (parts.Length < 7)
                {
                    Debug.LogWarning("Invalid standard dialogue line: " + line);
                    continue;
                }
                
                bool isPlayer = bool.Parse(parts[3].Trim());
                string expression = parts[4].Trim();
                string text = parts[5].Trim();
                if (text.StartsWith("\"") && text.EndsWith("\""))
                    text = text.Substring(1, text.Length - 2);

                string nextLineID = parts[6].Trim();
                if (string.IsNullOrEmpty(nextLineID)) nextLineID = null;

                
                DialogueLine dlgLine;

                if (parts.Length == 7)
                {
                    dlgLine = new DialogueLine(lineID, speakerName, isPlayer, expression, text, nextLineID);
                }
                else
                {
                    string newFlag = parts[7].Trim();
                    dlgLine = new DialogueLine(lineID, speakerName, isPlayer, expression, text, nextLineID, newFlag);
                }

                dialogueDict[lineID] = dlgLine;
            }
            else if (type == "choice")
            {
                // Choice line: lineID, choice, speakerName, true, [[expression, "text", nextLineID]], [[...]]
                // Find all [[...]] blocks
                string choiceBlocks = firstParts[4].Trim();
                if (choiceBlocks.StartsWith("~") && choiceBlocks.EndsWith("~"))
                    choiceBlocks = choiceBlocks.Substring(1, choiceBlocks.Length - 2);

                List<DialogueChoice> choices = new List<DialogueChoice>();

                foreach (string match in choiceBlocks.Split("||"))
                {
                    Debug.Log(match);

                    // Split by comma outside quotes
                    string[] parts = match.Split(new char[] { ';' });

                    if (parts.Length != 3)
                    {
                        Debug.LogWarning("Invalid choice block: " + match);
                        Debug.Log(choiceBlocks);
                        continue;
                    }

                    string expr = parts[0].Trim();
                    string txt = parts[1].Trim();
                    if (txt.StartsWith("\"") && txt.EndsWith("\""))
                        txt = txt.Substring(1, txt.Length - 2);

                    string nextID = parts[2].Trim();
                    if (string.IsNullOrEmpty(nextID)) nextID = null;

                    choices.Add(new DialogueChoice(expr, txt, nextID));
                }

                bool isPlayer = bool.Parse(firstParts[3].Trim()); // always true in your format
                DialogueLine dlgLine = new DialogueLine(lineID, speakerName, isPlayer, choices);
                dialogueDict[lineID] = dlgLine;
            }
            else if (type == "attack")
            {
                string encounterID = firstParts[2].Trim();
                string nextID = firstParts[3].Trim();
                string failID = firstParts[4].Trim();
                DialogueLine dlgLine = new DialogueLine(lineID, encounterID, nextID, failID);
                dialogueDict[lineID] = dlgLine;
            }
        }

        return dialogueDict;
    }
}
