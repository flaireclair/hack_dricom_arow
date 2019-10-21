using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


public class Fishing : MonoBehaviour
{

    private bool isFishing = false;
    private bool isHit = false;
    public GameObject Image;

    private float time = 0;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] target = GameObject.FindGameObjectsWithTag("River");
        Debug.Log(Objects.Fishes.Sheet1.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (Image.activeSelf && Time.time - time >= 3) Image.SetActive(false);
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 FishingPos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(AppUtil.GetTouchPosition());
            RaycastHit _hit = new RaycastHit();
            RaycastHit hit = new RaycastHit();
            if (!isFishing)
            {
                if (Physics.Raycast(ray, out _hit, 10.0f) && _hit.collider.gameObject.tag == "Ground")
                {
                    Debug.Log(true);
                    if (Physics.Raycast(_hit.collider.transform.position - new Vector3(0, -5f, 0), Vector3.down, out hit, 1000f) && hit.collider.gameObject.tag == "River")
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
        if(!Image.activeSelf) Image.SetActive(true);
        time = Time.time;
        Image.GetComponent<Image>().sprite = Resources.Load(string.Format("Usable_Fish/{0}/{0}", FishList[random].Name), typeof(Sprite)) as Sprite;
        foreach (FishEntity hoge in Player.fish)
        {
            Debug.Log(hoge.Name);
        }
    }
}
