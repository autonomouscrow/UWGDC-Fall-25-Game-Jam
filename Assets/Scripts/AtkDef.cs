using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class AtkDef
{
    public string id;            // Unique identifier
    public string name;
    public string flavTxt;       // Flavor text or description
    public AtkDefType type;      // Element/type (like Fire, Water, etc.)

    //truth; sus; happy; sad; confused; angry; sexy
    public List<float> vals = new List<float>();

    public AtkDef(string id, string name, string flavTxt, AtkDefType type, List<float> v)
    {
        this.id = id;
        this.name = name;
        this.flavTxt = flavTxt;
        this.type = type;
        this.vals = v;
    }
}

public enum AtkDefType
{
    standard,
    cheerful,
    confused,
    aggressive,
    sad,
    sexy,
    cubism
}
