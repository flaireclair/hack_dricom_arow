using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Fishing : MonoBehaviour
{

    private bool isFishing = false;
    private bool isHit = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] target = GameObject.FindGameObjectsWithTag("River");
        Debug.Log(Objects.Fishes.Sheet1.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (AppUtil.GetTouch() == TouchInfo.Began)
        {
            Vector3 FishingPos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(AppUtil.GetTouchPosition());
            RaycastHit hit = new RaycastHit();
            if (!isFishing)
            {
                if (Physics.Raycast(ray, out hit, 10.0f) && hit.collider.transform.tag == "Ground" && Physics.Raycast(hit.transform.position - new Vector3(0, -5f, 0), Vector3.down, out hit, 1000f) && hit.collider.gameObject.tag == "River")
                {
                    isFishing = true;
                    DateTime now = DateTime.Now;
                    if (now.Hour >= 6 && now.Hour <= 18)
                    {
                        Debug.Log("昼");
                    }
                    else
                    {
                        Debug.Log("夜");
                    }
                    StartCoroutine("WaitFish");
                }
            }
            else if(!isHit)
            {
                StopCoroutine("WaitFish");
                isFishing = false;
            }            
        }
    } 

    IEnumerator WaitFish()
    {
        float waitTime = UnityEngine.Random.Range(1.0f, 15.0f);
        Debug.Log(string.Format("Wait : {0}", waitTime));
        yield return new WaitForSeconds(waitTime);
        Debug.Log("Hit!");
        isHit = true;
        float fishingTime = Time.time;
        yield return new WaitWhile(() => AppUtil.GetTouch() == TouchInfo.None);
        if (Time.time - fishingTime < 2.0f) GetFish(DateTime.Now.Hour);
        Debug.Log(string.Format("Fishing : {0}", isFishing));
        isFishing = false;
        isHit = false;
        yield break;
    }
    
    private void GetFish(int hour)
    {
        Debug.Log("Yeah!");
        bool datetime = (hour >= 6 && hour <= 18);
        List<FishEntity> FishList = Objects.Fishes.Sheet1.FindAll(n => n.DateTime == datetime);
        int random = UnityEngine.Random.Range(0, Objects.Fishes.Sheet1.FindAll(n => n.DateTime == datetime).Count);
        Debug.Log(string.Format("Get Fish : {0}", FishList[random].Name));
        Player.fish.Add(FishList[random]);
        foreach(FishEntity hoge in Player.fish)
        {
            Debug.Log(hoge.Name);
        }
    }
}
