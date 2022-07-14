using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobSelectButton : MonoBehaviour
{
    private GameObject mobWrapper;

    private Button button;
    public string id;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(delegate { OnClick(); });

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
        mob.transform.localPosition = Vector3.zero;

        var debugMob = mob.AddComponent<DebugMob>();
        debugMob.Load(id);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
