using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishAndMoney : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RetuenMoney()
    {
        Transform canvas = transform.parent.GetChild(5);
        int getMoney = 0;
        foreach(FishData fish in StoreFishList.fishDataList)
        {
            for(int i = 0; i < int.Parse(canvas.GetChild(i).GetChild(2).GetChild(0).GetChild(2).GetComponent<Text>().text); i++)
            {
                Player.fish.Remove(fish.Fish);
                getMoney += fish.Fish.Money;
            }
        }

        Player.money += getMoney;
        foreach (FishEntity hoge in Player.fish)
        {
            Debug.Log(hoge.Name);
        }
        Debug.Log(string.Format("所持金 : {0}", Player.money));
    }
}
