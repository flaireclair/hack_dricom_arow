using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Fishing : MonoBehaviour
{

    private bool is_fishing = false;
    GameObject time;


    // Start is called before the first frame update
    void Start()
    {
        GameObject[] target = GameObject.FindGameObjectsWithTag("River");
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 FishingPos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(AppUtil.GetTouchPosition());
            RaycastHit hit = new RaycastHit();
            if (hit.collider.gameObject.tag == "River")
            {
                is_fishing = true;
                StartCoroutine("WaitFish");
            }
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

    IEnumerator WaitFish()
    {
        float waitTime = UnityEngine.Random.Range(1.0f, 30.0f);
        Debug.Log(string.Format("Wait : {0}", waitTime));
        yield return new WaitForSeconds(waitTime);
        float fishingTime = Time.deltaTime;
        yield return new WaitWhile(() => AppUtil.GetTouch() == TouchInfo.Canceled);
        if (Time.deltaTime - fishingTime < 2.0f) GetFish(DateTime.Now.Hour);
        yield return null;
    }
    
    private void GetFish(int hour)
    {
        Debug.Log("Yeah!");
        if(hour >= 6 && hour <= 18)
        {
            int fishID = UnityEngine.Random.Range(0, Objects.Fishes.Sheet1.Find();
        }
    }

}
