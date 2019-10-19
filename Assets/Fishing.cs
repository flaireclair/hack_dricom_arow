using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Fishing : MonoBehaviour
{

    private bool is_fishing = false;

    GameObject time;

    NowTime script;


    // Start is called before the first frame update
    void Start()
    {
        GameObject[] target = GameObject.FindGameObjectsWithTag("River");

        time = GameObject.Find("Time");

        script = time.GetComponent<NowTime>();


    }


    // Update is called once per frame
    void Update()
    {
       

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 FishingPos = Input.mousePosition;

            is_fishing = Throw(FishingPos);
        }

        Debug.Log(string.Format("Fishing : {0}", is_fishing));

        if(is_fishing)
        {
            DateTime now = DateTime.Now;

            if(now.Hour >= 6 && now.Hour <= 18)
            {
                Debug.Log("昼");
            }
            else
            {
                Debug.Log("夜");
            }
            
        }
    } 

    public bool Throw(Vector2 pos)
    {
        if (pos.x > 0)
        {
            return true;
        }
        return false;
    }

    
}
