using System.Collections.Generic;

[System.Serializable]
public class DialogueLine
{
    public string lineID;
    public string speakerName;
    public bool speakerIsPlayer;
    public bool isChoiceLine;
    public bool isAttackLine;

    public bool hasFlag = false;
    public string flag = "";

    // Used if standard line
    public string expression;
    public string text;
    public string nextLineID;
    public string failureLineID;

    // Used if choice line
    public List<DialogueChoice> choices;

    // attack lines
    public string encounterID;


    // Standard line constructor
    public DialogueLine(string id, string speaker, bool isPlayer, string expr, string txt, string next)
    {
        lineID = id;
        speakerName = speaker;
        speakerIsPlayer = isPlayer;
        isChoiceLine = false;
        expression = expr;
        text = txt;
        nextLineID = next;
        choices = null;
    }

    public DialogueLine(string id, string speaker, bool isPlayer, string expr, string txt, string next, string newFlag)
    {
        lineID = id;
        speakerName = speaker;
        speakerIsPlayer = isPlayer;
        isChoiceLine = false;
        expression = expr;
        text = txt;
        nextLineID = next;
        choices = null;
        hasFlag = true;
        flag = newFlag;
    }

    // Choice line constructor
    public DialogueLine(string id, string speaker, bool isPlayer, List<DialogueChoice> choiceList)
    {
        lineID = id;
        speakerName = speaker;
        speakerIsPlayer = isPlayer;
        isChoiceLine = true;
        choices = choiceList;
        expression = null;
        text = null;
        nextLineID = null;
    }

    //Attack line constructor
    public DialogueLine(string id, string encounter, string next, string fail)
    {
        lineID = id;
        speakerName = "[playername]";
        speakerIsPlayer = true;
        isChoiceLine = false;
        expression = "aha";
        text = "Aha! I've got you now!";
        encounterID = encounter;
        nextLineID = next;
        failureLineID = fail;
        isAttackLine = true;
    }
}
