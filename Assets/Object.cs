using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    public struct Fish
    {
        public int ID;
        public bool DateTime;
        public int Money;
        public string Name;
    }

    public struct Ship
    {
        public int ID;
        public int BuyingCost;
        public int ShippingCost;
        public string Name;
    }

    public struct Tool
    {
        public int ID;
        public int BuyingCost;
        public int Luck;
        public string Name;
    }

    public Fish A;
    // Start is called before the first frame update
    void Start()
    {
        A = new Fish() { ID = 0, DateTime = true, Money = 100, Name = "A" };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
