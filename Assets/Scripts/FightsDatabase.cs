using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;

public class FightsDatabase : MonoBehaviour
{
    public static FightData GetFightData(string encounterID)
    {
        TextAsset file = Resources.Load($"Fights/{encounterID}") as TextAsset;

        if (file == null)
        {
            Debug.LogError("heccc");
        }

        string[] lines = file.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        FightData ret = new FightData();

        ret.emotionMultipliers = lines[0].Split(';', 5).Select(s => float.Parse(s.Trim())).ToList();

        ret.lustEffect = (Meters)Enum.Parse(typeof(Meters), lines[1].Trim());
        ret.lustMultiplier = float.Parse(lines[2].Trim());

        int ind = 0;
        foreach (var line in lines)
        {
            if (ind >= 3)
            {
                // type; req; text; id; ~abid || abid || abid~; false;
                string[] parts = line.Split(";");
                if (parts.Length != 7)
                {
                    Debug.LogError(line + "not right length");
                }
                Note lineNote = new Note();

                lineNote.noteType = (NoteType)Enum.Parse(typeof(NoteType), parts[0].Trim());
                lineNote.noteReq = parts[1].Trim();
                lineNote.noteText = parts[2].Trim();
                lineNote.statementID = parts[3].Trim();

                string abilitiesList = parts[4].Trim();
                if (abilitiesList.StartsWith("~") && abilitiesList.EndsWith("~"))
                    abilitiesList = abilitiesList.Substring(1, abilitiesList.Length - 2);

                lineNote.abilityIDs = abilitiesList.Split("||").Select(s => s.Trim()).ToList();

                lineNote.setTruth = bool.Parse(parts[5].Trim());
                lineNote.TRUTHSET = parts[6].Trim()==""?0:float.Parse(parts[6].Trim());

                ret.notes.Add(lineNote);
            }
            ind++;
        }

        return ret;
    }
}
