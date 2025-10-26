[System.Serializable]
public class DialogueChoice
{
    public string expression;
    public string text;
    public string nextLineID;

    public DialogueChoice(string expr, string txt, string next)
    {
        expression = expr;
        text = txt;
        nextLineID = next;
    }
}
