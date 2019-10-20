using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void RetuenMoney(FishEntity fish, int num)
    {
        for(int i = 0; i < num; i++)
        {
            Player.fish.Remove(fish);
        }
        Player.money += fish.Money + num;
        foreach (FishEntity hoge in Player.fish)
        {
            Debug.Log(hoge.Name);
        }
        Debug.Log(string.Format("所持金 : {0}", Player.money));
    }
}
