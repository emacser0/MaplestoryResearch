using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using WzComparerR2.WzLib;

public class MobScrollView : MonoBehaviour
{
    public GameObject listElementPrefab;

    private MobLoader loader;
    private WzLoader wzLoader;

    private GameObject content;

    // Start is called before the first frame update
    void Start()
    {
        loader = GameObject.Find("Loader").GetComponent<MobLoader>();
        wzLoader = GameObject.Find("Loader").GetComponent<WzLoader>();

        content = transform.Find("Viewport").Find("Content").gameObject;
        RectTransform contentTransform = content.GetComponent<RectTransform>();        

        foreach(Wz_Node node in wzLoader["Mob"].WzNode.Nodes)
        {
            GameObject listElement = Instantiate(listElementPrefab, content.transform);
            listElement.name = node.Text;

            RectTransform rectTransform = listElement.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(1, 20);

            Text text = listElement.transform.Find("Text").GetComponent<Text>();
            text.text = node.Text;

            contentTransform.sizeDelta = new Vector2(contentTransform.sizeDelta.x, contentTransform.sizeDelta.y + 20);
        }
    }    

    // Update is called once per frame
    void Update()
    {
        
    }
}
