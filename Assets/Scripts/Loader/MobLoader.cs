using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class WzSprite : WzData
{
    public WzSprite()
    {

    }

    public WzSprite(XmlElement element)
        : base(element)
    {
        _texture = new Texture2D(1, 1);
        if(element.Name == "uol")
        {
            Vector["size"] = new Vector2Int(1, 1);
            String["_uollink"] = element.GetAttribute("value");
            return;
        }

        string imageString = element.GetAttribute("value");
        byte[] imageBytes = System.Convert.FromBase64String(imageString);

        _texture.LoadImage(imageBytes);
        _texture.wrapMode = TextureWrapMode.Clamp;

        _sprite = UnityEngine.Sprite.Create(_texture, new Rect(0.0f, 0.0f, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), 100.0f);

        Vector["size"] = new Vector2Int(_texture.width, _texture.height);
    }

    private Texture2D _texture;
    private Sprite _sprite;

    public Texture2D texture { get => _texture; set => _texture = value; }
    public Sprite sprite { get => _sprite; set => _sprite = value; }
}

public class MobLoader : MonoBehaviour
{
    // Start is called before the first frame update

    private Dictionary<string, WzData> _cache = new Dictionary<string, WzData>();

    void Start()
    {

    }

    public WzData Load(string mobId)
    {
        if(_cache.ContainsKey(mobId))
        {
            return _cache[mobId];
        }

        string filePath = Application.dataPath + "/Mobs/" + mobId + ".xml";

        XmlDocument doc = new XmlDocument();
        doc.Load(filePath);

        WzData mob = new WzData(doc["dir"]);
        _cache[mobId] = mob;

        return mob;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
