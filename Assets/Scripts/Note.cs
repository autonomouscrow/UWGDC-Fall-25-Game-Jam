using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public enum NoteType
{
    text,
    statement,
    ability,
    abilityClear //only used by the duck
}
[System.Serializable]
public class Note
{
    public NoteType noteType;
    public string noteReq; // happy, sad, angry, confused, neutral, etc.

    public string noteText;
    public string statementID;
    public List<string> abilityIDs = new List<string>();

    public bool setTruth = false; //use only when truth meter needs to be set directly
    public float TRUTHSET;
}
