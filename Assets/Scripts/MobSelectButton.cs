using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobSelectButton : MonoBehaviour
{
    private GameObject mobWrapper;

    private Button button;
    private Text text;    

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(delegate { OnClick(); });

        text = transform.Find("Text").GetComponent<Text>();
        mobWrapper = GameObject.Find("MobWrapper");
    }
    private void OnClick()
    {
        var mobTransform = mobWrapper.transform.Find("Mob");
        if (mobTransform != null)
        {
            Destroy(mobTransform.gameObject);
        }

        var mob = new GameObject("Mob");
        mob.transform.parent = mobWrapper.transform;

        var debugMob = mob.AddComponent<DebugMob>();
        debugMob.Load(text.text.Split(".")[0]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
