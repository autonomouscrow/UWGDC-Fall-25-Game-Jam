using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FightData
{
    public List<Note> notes = new List<Note>();
    public List<float> emotionMultipliers; // sus, happy, sad, confused, angry

    public Meters lustEffect;
    public float lustMultiplier;
}
