using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Drawing.Imaging;

using UnityEngine;
using WzComparerR2.WzLib;

internal class SpriteObject
{
    public SpriteObject()
    {

    }

    public SpriteObject(string name, Transform parent)
    {
        var spriteTransform = parent.Find(name);
        if (spriteTransform == null)
        {
            gameObject = new GameObject(name);
            gameObject.transform.parent = parent;
        }
        else
        {
            gameObject = spriteTransform.gameObject;
        }

        renderer = gameObject.GetComponent<SpriteRenderer>();
        if (gameObject.GetComponent<SpriteRenderer>() == null)
        {
            renderer = gameObject.AddComponent<SpriteRenderer>();
        }
        gameObject.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Sliced;

        collider = gameObject.GetComponent<BoxCollider2D>();
        if (gameObject.GetComponent<BoxCollider2D>() == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }

        transform = gameObject.GetComponent<RectTransform>();
        if (transform == null)
        {
            transform = gameObject.AddComponent<RectTransform>();
        }
    }

    public GameObject gameObject;
    public SpriteRenderer renderer;
    public BoxCollider2D collider;
    public RectTransform transform;
}

public class DebugMob : MonoBehaviour
{
    public string id = "";
    public string currentAnimationType = "stand";

    int currentSpriteIndex = 0;
    int currentEffectIndex = 0;

    SpriteObject character = null;
    SpriteObject effect = null;

    private WzLoader wzLoader = null;
    private WzMob wzMob = null;

    private Dictionary<string, Wz_Node> _animationNodeCache = new Dictionary<string, Wz_Node>();
    private Dictionary<string, Wz_Node> _pngNodeCache = new Dictionary<string, Wz_Node>();
    private Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>();

    private Coroutine animationCoroutine = null;
    private Coroutine characterCoroutine = null;
    private Coroutine effectCoroutine = null;

    // Start is called before the first frame update
    void Awake()
    {
        wzLoader = GameObject.Find("Loader").GetComponent<WzLoader>();

        var preview = transform.Find("Preview");
        if(preview != null)
        {
            Destroy(preview.gameObject);
        }
       
        if(id.Length > 0)
        {
            Load(id);
        }
    }

    public void Load(string mobId)
    {
        _animationNodeCache.Clear();
        _pngNodeCache.Clear();
        _spriteCache.Clear();

        id = mobId;

        character = new SpriteObject("Character", transform);
        effect = new SpriteObject("Effect", transform);

        effect.transform.localPosition = new Vector3(
            effect.transform.localPosition.x,
            effect.transform.localPosition.y,
            -1.0f);

        currentSpriteIndex = 0;
        currentEffectIndex = 0;

        var wzImg = wzLoader.GetImg("Mob", mobId);
        wzMob = new WzMob(wzImg);

        animationCoroutine = StartCoroutine(Animate());
    }

    public void SetAnimationType(string animationType)
    {
        currentAnimationType = animationType;
        currentSpriteIndex = 0;
        currentEffectIndex = 0;

        animationCoroutine = StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        while(true)
        {
            characterCoroutine = StartCoroutine(AnimateCharacter());
            effectCoroutine = StartCoroutine(AnimateEffect());

            yield return characterCoroutine;
            yield return effectCoroutine;

            currentSpriteIndex = 0;
            currentEffectIndex = 0;
        }
    }

    private Wz_Node GetAnimationNode(string animationType)
    {
        Wz_Node animationNode = null;
        if (_animationNodeCache.ContainsKey(animationType))
        {
            animationNode = _animationNodeCache[animationType];
        }
        else
        {
            var infoNode = wzMob.root.Nodes["info"];
            if (infoNode != null)
            {
                var linkNode = infoNode.Nodes["link"];
                if (linkNode != null)
                {
                    var linkedImg = wzLoader.GetImg("Mob", linkNode.Value.ToString());
                    var linkedNode = linkedImg.Node;
                    animationNode = linkedNode.Nodes[animationType];
                }
                else
                {
                    animationNode = wzMob.root.Nodes[animationType];
                }
            }

            _animationNodeCache[animationType] = animationNode;
        }

        return animationNode;
    }

    private int GetAnimationLength(Wz_Node animationNode)
    {
        int length = 0;
        foreach(Wz_Node subNode in animationNode.Nodes)
        {
            int outIndex;
            if(int.TryParse(subNode.Text, out outIndex))
            {
                ++length;
            }
        }

        return length;
    }

    Wz_Node GetPngNode(Wz_Node animationNode)
    {
        string fullpath = animationNode.FullPath + "\\" + currentSpriteIndex.ToString();
        if (_pngNodeCache.ContainsKey(fullpath))
        {
            return _pngNodeCache[fullpath];
        }
        else
        {
            Wz_Node wzSpriteNode = animationNode.Nodes[currentSpriteIndex.ToString()];
            Wz_Node wzPngNode = wzSpriteNode;
            var valueType = wzSpriteNode.Value.GetType();

            if (valueType == typeof(Wz_Uol))
            {
                Wz_Uol uol = (Wz_Uol)wzSpriteNode.Value;
                Wz_Node uolTargetNode = uol.HandleUol(wzSpriteNode);

                if (uolTargetNode.Value.GetType() == typeof(Wz_Png))
                {
                    wzPngNode = uolTargetNode;
                }
            }

            _pngNodeCache[fullpath] = wzPngNode;
            return wzPngNode;
        }        
    }

    Wz_Png GetPngFromNode(Wz_Node pngNode)
    {
        return (Wz_Png)pngNode.Value;
    }

    Sprite GetSprite(Wz_Node pngNode)
    {
        var png = GetPngFromNode(pngNode);

        if (_spriteCache.ContainsKey(pngNode.FullPath))
        {
            return _spriteCache[pngNode.FullPath];
        }
        else
        {
            var inlinkNode = pngNode.Nodes["_inlink"];
            var outlinkNode = pngNode.Nodes["_outlink"];


            if (inlinkNode != null)
            {
                var inlink = (string)inlinkNode.Value;
                var fullpath = inlink.Replace('/', '\\');
                var node = wzMob.root.FindNodeByPath(fullpath, true);

                png = (Wz_Png)node.Value;
            }

            if (outlinkNode != null)
            {
                var outlink = (string)outlinkNode.Value;
                var tokens = outlink.Split("/");
                var wzDirName = tokens[0];
                var wzImgName = tokens[1];

                var fullpath = "";
                for (int i = 2; i < tokens.Length; i++)
                {
                    fullpath += tokens[i];

                    if (i < tokens.Length - 1)
                    {
                        fullpath += "\\";
                    }
                }

                var img = wzLoader.GetImg("Mob", wzImgName.Split(".")[0]);
                var targetNode = img.Node.FindNodeByPath(fullpath);
                png = (Wz_Png)targetNode.Value;
            }

            var bitmap = png.ExtractPng();
            var texture = new Texture2D(bitmap.Width, bitmap.Height);

            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            var buffer = new byte[ms.Length];

            ms.Position = 0;
            ms.Read(buffer, 0, buffer.Length);
            
            texture.LoadImage(buffer);
            texture.Apply();

            texture.wrapMode = TextureWrapMode.Clamp;

            var sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

            _spriteCache[pngNode.FullPath] = sprite;
            return sprite;
        }
    }

    private IEnumerator AnimateCharacter()
    {
        var animationNode = GetAnimationNode(currentAnimationType);
        var animationLength = GetAnimationLength(animationNode);

        while (currentSpriteIndex < animationLength)
        {
            var wzPngNode = GetPngNode(animationNode);
            var wzPng = GetPngFromNode(wzPngNode);

            Sprite sprite = GetSprite(wzPngNode);

            character.renderer.sprite = sprite;
            character.renderer.size = new Vector2Int(sprite.texture.width, sprite.texture.height);

            var origin = character.renderer.size / 2;

            var wzOriginNode = wzPngNode.Nodes["origin"];
            if (wzOriginNode != null)
            {
                Wz_Vector wzOrigin = (Wz_Vector)wzOriginNode.Value;

                Wz_Node leftTopNode = wzPngNode.Nodes["lt"];
                Wz_Node rightBottomNode = wzPngNode.Nodes["rb"];

                Wz_Vector wzLeftTop = leftTopNode != null ? (Wz_Vector)leftTopNode.Value : wzOrigin;
                Wz_Vector wzRightBottom = rightBottomNode != null ? (Wz_Vector)rightBottomNode.Value : wzOrigin;

                var leftTop = new Vector2Int(wzOrigin.X + wzLeftTop.X, wzOrigin.Y + wzLeftTop.Y);
                var rightBottom = new Vector2Int(wzOrigin.X + wzRightBottom.X, wzOrigin.Y + wzRightBottom.Y);
                var center = (leftTop + rightBottom) / 2;

                character.transform.localPosition = new Vector3(
                    origin.x - wzOrigin.X, wzOrigin.Y - origin.y, 0.0f);

                character.collider.offset = new Vector2(center.x - origin.x, origin.y - center.y);
                character.collider.size = rightBottom - leftTop;
            }

            Wz_Node delayNode = wzPngNode.Nodes["delay"];
            if (delayNode != null)
            {
                yield return new WaitForSeconds((int)delayNode.Value / 1000.0f);
            }
            else
            {
                yield return new WaitForSeconds(90 / 1000.0f);
            }

            currentSpriteIndex = currentSpriteIndex + 1;
        }

        yield return null;
    }

    private IEnumerator AnimateEffect()
    {
        var animationNode = GetAnimationNode(currentAnimationType);

        var infoNode = animationNode.Nodes["info"];
        if(infoNode != null)
        {
            var effectNode = infoNode.Nodes["effect"];
            if (effectNode != null)
            {
                var effectAfterNode = infoNode.Nodes["effectAfter"];
                if (effectAfterNode != null)
                {
                    yield return new WaitForSeconds((int)effectAfterNode.Value / 1000.0f);
                }

                effect.gameObject.SetActive(true);
                while (currentEffectIndex < effectNode.Nodes.Count)
                {
                    var wzPngNode = GetPngNode(effectNode);
                    var wzPng = GetPngFromNode(wzPngNode);

                    Sprite sprite = GetSprite(wzPngNode);

                    effect.renderer.sprite = sprite;
                    effect.renderer.size = new Vector2Int(sprite.texture.width, sprite.texture.height);

                    var origin = effect.renderer.size / 2;

                    var wzOriginNode = wzPngNode.Nodes["origin"];
                    if (wzOriginNode != null)
                    {
                        Wz_Vector wzOrigin = (Wz_Vector)wzOriginNode.Value;

                        effect.transform.localPosition = new Vector3(
                            origin.x - wzOrigin.X, wzOrigin.Y - origin.y, -1.0f);
                    }

                    Wz_Node delayNode = wzPngNode.Nodes["delay"];
                    if (delayNode != null)
                    {
                        yield return new WaitForSeconds((int)delayNode.Value / 1000.0f);
                    }
                    else
                    {
                        yield return new WaitForSeconds(90 / 1000.0f);
                    }

                    currentEffectIndex = currentEffectIndex + 1;
                }
                effect.gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
