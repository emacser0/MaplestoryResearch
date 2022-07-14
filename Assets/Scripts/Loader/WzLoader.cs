using System.Collections.Generic;
using UnityEngine;
using WzComparerR2.WzLib;

internal class WzLoaderCache
{
    public Wz_Structure structure;
    public Dictionary<string, Wz_Image> images = new Dictionary<string, Wz_Image>();
}

public class WzLoader : MonoBehaviour
{
    private Dictionary<string, WzLoaderCache> _cache = new Dictionary<string, WzLoaderCache>();
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

    private void LoadWz(string wzName)
    {
        if(_cache.ContainsKey(wzName))
        {
            return;
        }
        Wz_Structure structure = new Wz_Structure();
        structure.LoadKMST1125DataWz("Data\\" + wzName + "\\" + wzName + ".wz");

        _cache[wzName] = new WzLoaderCache();      
        _cache[wzName].structure = structure;

        return;
    }

    private WzLoaderCache GetCache(string wzName)
    {
        if(_cache.ContainsKey(wzName))
        {
            return _cache[wzName];
        }

        LoadWz(wzName);

        return _cache[wzName];
    }

    public Wz_Structure GetWz(string wzName)
    {
        if (_cache.ContainsKey(wzName))
        {
            return _cache[wzName].structure;
        }

        LoadWz(wzName);

        return _cache[wzName].structure;
    }

    public Wz_Image GetImg(string wzName, string id)
    {
        WzLoaderCache cache = GetCache(wzName);
        if(cache.images.ContainsKey(id))
        {
            return cache.images[id];
        }

        Wz_Node node = null;

        foreach (Wz_Node subNode in cache.structure.WzNode.Nodes)
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

        cache.images[id] = img;

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
