using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{

    public GameObject money_object = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Text money_text = money_object.GetComponent<Text>();

        money_text.text = "" + Player.money;
    }
}
