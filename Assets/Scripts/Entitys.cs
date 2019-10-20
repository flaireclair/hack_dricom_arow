using UnityEngine;   

[System.Serializable]
public class FishEntity
{
    public int ID;
    public bool DateTime;
    public int Money;
    public string Name;
    
}

[System.Serializable]
public class ShipEntity
{
    public int ID;
    public int BuyingCost;
    public int ShippingCost;
    public string Name;
}

[System.Serializable]
public class ToolEntity
{
    public int ID;
    public int BuyingCost;
    public int Luck;
    public string Name;
}

[System.Serializable]
public class FishData
{
    public FishEntity Fish;
    public int Num;
    public Sprite Image;
}
