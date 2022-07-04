using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Loader;

public class SpriteObject
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
    public string id;
    public string animationType = "stand";

    private MobLoader loader;
    private WzData mob;

    int currentSpriteIndex = 0;
    int currentEffectIndex = 0;

    SpriteObject character;
    SpriteObject effect;

    string currentAnimationType;

    // Start is called before the first frame update
    void Start()
    {
        loader = GameObject.Find("Loader").GetComponent<MobLoader>();
        mob = loader.Load(id);

        var preview = transform.Find("Preview");
        if(preview != null)
        {
            Destroy(preview.gameObject);
        }

        character = new SpriteObject("Character", transform);
        effect = new SpriteObject("Effect", transform);

        effect.transform.localPosition = new Vector3(
            effect.transform.localPosition.x,
            effect.transform.localPosition.y,
            -1.0f);

        StartCoroutine("Animate");
    }

    private IEnumerator Animate()
    {
        while(true)
        {
            Coroutine character = StartCoroutine(AnimateCharacter());
            Coroutine effect = StartCoroutine(AnimateEffect());

            yield return character;
            yield return effect;

            currentSpriteIndex = 0;
            currentEffectIndex = 0;
        }
    }

    private IEnumerator AnimateCharacter()
    {
        WzData animationSprite = null;
        if(mob.Dir["info"].HasString("link"))
        {
            animationSprite = loader.Load(mob.Dir["info"].String["link"]).Dir[animationType];
        }
        else
        {
            animationSprite = mob.Dir[animationType];
        }

        while (currentSpriteIndex < animationSprite.Sprite.Count)
        {
            var wzSprite = animationSprite.Sprite[currentSpriteIndex.ToString()];
            var sprite = wzSprite.sprite;
            var spriteSize = wzSprite.Vector["size"];

            if(wzSprite.HasString("_inlink"))
            {
                var link = wzSprite.String["_inlink"];
                var linkTokens = link.Split("/");

                var targetWzSprite = mob.Dir[linkTokens[0]].Sprite[linkTokens[1]];

                sprite = targetWzSprite.sprite;
                spriteSize = targetWzSprite.Vector["size"];
            }
            else if(wzSprite.HasString("_outlink"))
            {
                var link = wzSprite.String["_outlink"];
                var linkTokens = link.Split("/");

                var mobId = linkTokens[1].Split(".")[0];

                var targetWzSprite = loader.Load(mobId).Dir[linkTokens[2]].Sprite[linkTokens[3]];

                sprite = targetWzSprite.sprite;
                spriteSize = targetWzSprite.Vector["size"];
            }
            else if(wzSprite.HasString("_uollink"))
            {
                var link = wzSprite.String["_uollink"];
                var linkTokens = link.Split("/");

                if (linkTokens.Length == 1)
                {
                    wzSprite = animationSprite.Sprite[linkTokens[0]];
                    sprite = wzSprite.sprite;
                }
                else if(linkTokens.Length == 3)
                {
                    wzSprite = mob.Dir[linkTokens[1]].Sprite[linkTokens[2]];
                    sprite = wzSprite.sprite;
                }

                spriteSize = wzSprite.Vector["size"];
            }

            character.renderer.sprite = sprite;
            character.renderer.size = spriteSize;

            var origin = character.renderer.size / 2;

            if (wzSprite.HasVector("origin"))
            {
                var wzOrigin = wzSprite.Vector["origin"];
                var wzLeftTop = wzOrigin;
                var wzRightBottom = wzOrigin;

                if (wzSprite.HasVector("lt"))
                {
                    wzLeftTop = wzSprite.Vector["lt"];
                }
                if (wzSprite.HasVector("rb"))
                {
                    wzRightBottom = wzSprite.Vector["rb"];
                }

                var leftTop = wzOrigin + wzLeftTop;
                var rightBottom = wzOrigin + wzRightBottom;
                var center = (leftTop + rightBottom) / 2;

                character.transform.localPosition = new Vector3(
                    origin.x - wzOrigin.x, wzOrigin.y - origin.y, 0.0f);

                character.collider.offset = new Vector2(center.x - origin.x, origin.y - center.y);
                character.collider.size = rightBottom - leftTop;
            }

            if (wzSprite.HasInt("delay"))
            {
                yield return new WaitForSeconds(wzSprite.Int["delay"] / 1000.0f);
            }
            else
            {
                yield return new WaitForSeconds(90 / 1000.0f);
            }

            currentSpriteIndex = currentSpriteIndex + 1;
        }
    }

    private IEnumerator AnimateEffect()
    {
        WzData animationSprite = null;
        if (mob.Dir["info"].HasString("link"))
        {
            animationSprite = loader.Load(mob.Dir["info"].String["link"]).Dir[animationType];
        }
        else
        {
            animationSprite = mob.Dir[animationType];
        }

        if (animationSprite.HasDir("info"))
        {
            var animationInfo = animationSprite.Dir["info"];
            if (animationInfo.HasDir("effect"))
            {
                var effectAnimation = animationInfo.Dir["effect"];
                if (animationInfo.HasInt("effectAfter"))
                {
                    yield return new WaitForSeconds(animationInfo.Int["effectAfter"] / 1000.0f);
                }

                effect.gameObject.SetActive(true);
                while (currentEffectIndex < effectAnimation.Sprite.Count)
                {
                    var wzSprite = effectAnimation.Sprite[System.Convert.ToString(currentEffectIndex)];
                    var sprite = wzSprite.sprite;
                    var spriteSize = wzSprite.Vector["size"];

                    if (wzSprite.HasString("_inlink"))
                    {
                        var link = wzSprite.String["_inlink"];
                        var linkTokens = link.Split("/");

                        var targetWzSprite = mob.Dir[linkTokens[0]].Dir[linkTokens[1]].Dir[linkTokens[2]].Sprite[linkTokens[3]];

                        sprite = targetWzSprite.sprite;
                        spriteSize = targetWzSprite.Vector["size"];
                    }
                    else if (wzSprite.HasString("_outlink"))
                    {
                        var link = wzSprite.String["_outlink"];
                        var linkTokens = link.Split("/");

                        var mobId = linkTokens[1].Split(".")[0];

                        var targetWzSprite = loader.Load(mobId).Dir[linkTokens[2]].Dir[linkTokens[3]].Dir[linkTokens[4]].Sprite[linkTokens[5]];

                        sprite = targetWzSprite.sprite;
                        spriteSize = targetWzSprite.Vector["size"];
                    }
                    else if (wzSprite.HasString("_uollink"))
                    {
                        var link = wzSprite.String["_uollink"];
                        var linkTokens = link.Split("/");

                        if (linkTokens.Length == 1)
                        {
                            wzSprite = animationSprite.Sprite[linkTokens[0]];
                        }
                        else if (linkTokens.Length == 3)
                        {
                            wzSprite = mob.Dir[linkTokens[1]].Sprite[linkTokens[2]];
                        }

                        sprite = wzSprite.sprite;
                        spriteSize = wzSprite.Vector["size"];
                    }

                    effect.renderer.sprite = sprite;
                    effect.renderer.size = spriteSize;

                    if (wzSprite.HasVector("origin"))
                    {
                        var origin = effect.renderer.size / 2;
                        var wzOrigin = wzSprite.Vector["origin"];

                        effect.transform.localPosition = new Vector3(
                            origin.x - wzOrigin.x, wzOrigin.y - origin.y, -1.0f);
                    }

                    if (wzSprite.HasInt("delay"))
                    {
                        yield return new WaitForSeconds(wzSprite.Int["delay"] / 1000.0f);
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
