using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NowTime : MonoBehaviour
{

    public int hour;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        DateTime now = DateTime.Now;

       int hour = now.Hour;
        int minute = now.Minute;           
        int second = now.Second;


        Debug.Log(string.Format("{0}時{1}分{2}秒", hour, minute, second));

    }

    
}
