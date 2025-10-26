using UnityEngine;

[System.Serializable]
public class TalkSprite
{
    public string name;   // e.g. "neutral", "happy", "angry"
    public Sprite sprite; // portrait sprite
}

public class TalkSprites : MonoBehaviour
{
    public TalkSprite[] expressions;

    public Sprite GetSprite(string name)
    {
        foreach (var expr in expressions)
        {
            if (expr.name == name)
                return expr.sprite;
        }
        return null; // not found
    }

    public Sprite GetSprite(int index)
    {
        if (index >= 0 && index < expressions.Length)
            return expressions[index].sprite;
        return null;
    }
}
