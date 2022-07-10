using System.Collections.Generic;
using UnityEngine;
using WzComparerR2.WzLib;

public class WzLoader : MonoBehaviour
{
    private Dictionary<string, Wz_Structure> _structures = new Dictionary<string, Wz_Structure>();
    private Dictionary<string, bool> _loaded = new Dictionary<string, bool>();
    private string[] wzFiles =
    {
        "Character",
        "Effect",
        "Etc",
        "Item",
        "Map",
        "Mob",
        "Morph",
        "Npc",
        "Quest",
        "Reactor",
        "Skill",
        "Sound",
        "String",
        "TamingMob",
        "UI"
    };

    private Wz_Node _node;

    void Start()
    {

    }

    public Wz_Structure GetWz(string wzName)
    {
        if (_structures.ContainsKey(wzName))
        {
            return _structures[wzName];
        }

        Wz_Structure structure = new Wz_Structure();
        structure.LoadKMST1125DataWz("Data\\" + wzName + "\\" + wzName + ".wz");
        _structures[wzName] = structure;

        return structure;
    }

    public Wz_Image GetImg(string wzName, string id)
    {
        Wz_Structure structure = GetWz(wzName);

        Wz_Node node = null;

        foreach (Wz_Node subNode in structure.WzNode.Nodes)
        {
            if (subNode.Text == id + ".img")
            {
                node = subNode;
            }
        }

        if (node == null)
        {
            return null;
        }

        Wz_Image img = (Wz_Image)node.Value;
        img.TryExtract();

        return img;
    }

    public Wz_Structure this[string wzName]
    {
        get { return GetWz(wzName); }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
