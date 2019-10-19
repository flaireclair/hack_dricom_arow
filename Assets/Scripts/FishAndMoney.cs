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

    public void RetuenMoney(int id)
    {
        Player.money += id * 100 + 100;
    }
}
