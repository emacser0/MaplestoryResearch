using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;


public class WzData
{
    public WzData()
    {

    }

    public WzData(XmlElement element)
    {
        XmlLoadInt(element);
        XmlLoadSingle(element);
        XmlLoadDouble(element);
        XmlLoadString(element);
        XmlLoadVector(element);
        XmlLoadSprite(element);
        XmlLoadDir(element);
    }

    public void XmlLoadInt(XmlElement element)
    {
        foreach (XmlElement property in element.GetElementsByTagName("int32"))
        {
            if (property.ParentNode != element)
            {
                continue;
            }

            string propertyName = property.GetAttribute("name");
            string propertyValue = property.GetAttribute("value");
            int propertyValueInt = System.Convert.ToInt32(propertyValue);

            Int[propertyName] = propertyValueInt;
        }
    }

    public void XmlLoadSingle(XmlElement element)
    {
        foreach (XmlElement property in element.GetElementsByTagName("single"))
        {
            if (property.ParentNode != element)
            {
                continue;
            }

            string propertyName = property.GetAttribute("name");
            string propertyValue = property.GetAttribute("value");
            float propertyValueSingle = System.Convert.ToInt32(propertyValue);

            Single[propertyName] = propertyValueSingle;
        }
    }

    public void XmlLoadDouble(XmlElement element)
    {
        foreach (XmlElement property in element.GetElementsByTagName("double"))
        {
            if (property.ParentNode != element)
            {
                continue;
            }

            string propertyName = property.GetAttribute("name");
            string propertyValue = property.GetAttribute("value");
            double propertyValueDouble = System.Convert.ToInt32(propertyValue);

            Double[propertyName] = propertyValueDouble;
        }
    }

    public void XmlLoadString(XmlElement element)
    {
        foreach (XmlElement property in element.GetElementsByTagName("string"))
        {
            if (property.ParentNode != element)
            {
                continue;
            }

            string propertyName = property.GetAttribute("name");
            string propertyValue = property.GetAttribute("value");

            String[propertyName] = propertyValue;
        }
    }

    public void XmlLoadVector(XmlElement element)
    {
        foreach (XmlElement property in element.GetElementsByTagName("vector"))
        {
            if (property.ParentNode != element)
            {
                continue;
            }

            string propertyName = property.GetAttribute("name");
            string propertyValue = property.GetAttribute("value");
            string[] propertyValueTokens = propertyValue.Split(",");
            Vector2Int propertyValueVector = new Vector2Int(
                System.Convert.ToInt32(propertyValueTokens[0]),
                System.Convert.ToInt32(propertyValueTokens[1]));

            Vector[propertyName] = propertyValueVector;
        }
    }

    public void XmlLoadSprite(XmlElement element)
    {
        foreach (XmlElement property in element.GetElementsByTagName("png"))
        {
            if (property.ParentNode != element)
            {
                continue;
            }

            string propertyName = property.GetAttribute("name");
            string propertyValue = property.GetAttribute("value");

            Sprite[propertyName] = new WzSprite(property);
        }

        foreach (XmlElement property in element.GetElementsByTagName("uol"))
        {
            if (property.ParentNode != element)
            {
                continue;
            }

            string propertyName = property.GetAttribute("name");
            string propertyValue = property.GetAttribute("value");

            Sprite[propertyName] = new WzSprite(property);
        }
    }

    public void XmlLoadDir(XmlElement element)
    {
        foreach (XmlElement property in element.GetElementsByTagName("dir"))
        {
            if (property.ParentNode != element)
            {
                continue;
            }

            string propertyName = property.GetAttribute("name");
            string propertyValue = property.GetAttribute("value");

            Dir[propertyName] = new WzData(property);
        }
    }

    public int GetInt(string key)
    {
        return _intProperties[key];
    }

    public float GetSingle(string key)
    {
        return _singleProperties[key];
    }

    public double GetDouble(string key)
    {
        return _doubleProperties[key];
    }

    public string GetString(string key)
    {
        return _stringProperties[key];
    }

    public Vector2Int GetVector(string key)
    {
        return _vectorProperties[key];
    }

    public WzSprite GetSprite(string key)
    {
        return _spriteProperties[key];
    }

    public WzData GetDir(string key)
    {
        return _dirProperties[key];
    }

    public void SetInt(string key, int value)
    {
        _intProperties[key] = value;
    }

    public void SetSingle(string key, float value)
    {
        _singleProperties[key] = value;
    }

    public void SetDouble(string key, double value)
    {
        _doubleProperties[key] = value;
    }

    public void SetString(string key, string value)
    {
        _stringProperties[key] = value;
    }

    public void SetVector(string key, Vector2Int value)
    {
        _vectorProperties[key] = value;
    }

    public void SetSprite(string key, WzSprite value)
    {
        _spriteProperties[key] = value;
    }

    public void SetDir(string key, WzData value)
    {
        _dirProperties[key] = value;
    }

    public bool HasInt(string key)
    {
        return _intProperties.ContainsKey(key);
    }

    public bool HasSingle(string key)
    {
        return _singleProperties.ContainsKey(key);
    }

    public bool HasDouble(string key)
    {
        return _doubleProperties.ContainsKey(key);
    }

    public bool HasString(string key)
    {
        return _stringProperties.ContainsKey(key);
    }

    public bool HasVector(string key)
    {
        return _vectorProperties.ContainsKey(key);
    }

    public bool HasSprite(string key)
    {
        return _spriteProperties.ContainsKey(key);
    }

    public bool HasDir(string key)
    {
        return _dirProperties.ContainsKey(key);
    }

    public Dictionary<string, int>.Enumerator GetIntEnumerator()
    {
        return _intProperties.GetEnumerator();
    }

    public Dictionary<string, float>.Enumerator GetSingleEnumerator()
    {
        return _singleProperties.GetEnumerator();
    }

    public Dictionary<string, double>.Enumerator GetDoubleEnumerator()
    {
        return _doubleProperties.GetEnumerator();
    }

    public Dictionary<string, string>.Enumerator GetStringEnumerator()
    {
        return _stringProperties.GetEnumerator();
    }

    public Dictionary<string, Vector2Int>.Enumerator GetVectorEnumerator()
    {
        return _vectorProperties.GetEnumerator();
    }

    public Dictionary<string, WzSprite>.Enumerator GetSpriteEnumerator()
    {
        return _spriteProperties.GetEnumerator();
    }

    public Dictionary<string, WzData>.Enumerator GetDirEnumerator()
    {
        return _dirProperties.GetEnumerator();
    }

    public int GetIntCount()
    {
        return _intProperties.Count;
    }

    public int GetSingleCount()
    {
        return _singleProperties.Count;
    }

    public int GetDoubleCount()
    {
        return _doubleProperties.Count;
    }

    public int GetStringCount()
    {
        return _stringProperties.Count;
    }

    public int GetVectorCount()
    {
        return _vectorProperties.Count;
    }

    public int GetSpriteCount()
    {
        return _spriteProperties.Count;
    }

    public int GetDirCount()
    {
        return _dirProperties.Count;
    }

    private Dictionary<string, int> _intProperties = new Dictionary<string, int>();
    private Dictionary<string, float> _singleProperties = new Dictionary<string, float>();
    private Dictionary<string, double> _doubleProperties = new Dictionary<string, double>();
    private Dictionary<string, string> _stringProperties = new Dictionary<string, string>();
    private Dictionary<string, Vector2Int> _vectorProperties = new Dictionary<string, Vector2Int>();
    private Dictionary<string, WzSprite> _spriteProperties = new Dictionary<string, WzSprite>();
    private Dictionary<string, WzData> _dirProperties = new Dictionary<string, WzData>();

    public Dictionary<string, int> Int { get => _intProperties; set => _intProperties = value; }
    public Dictionary<string, float> Single { get => _singleProperties; set => _singleProperties = value; }
    public Dictionary<string, double> Double { get => _doubleProperties; set => _doubleProperties = value; }
    public Dictionary<string, string> String { get => _stringProperties; set => _stringProperties = value; }
    public Dictionary<string, Vector2Int> Vector { get => _vectorProperties; set => _vectorProperties = value; }
    public Dictionary<string, WzSprite> Sprite { get => _spriteProperties; set => _spriteProperties = value; }
    public Dictionary<string, WzData> Dir { get => _dirProperties; set => _dirProperties = value; }
}
