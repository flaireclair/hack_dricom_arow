using System.Collections;
using System.Collections.Generic;
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

public class Objects : MonoBehaviour
{
    [SerializeField]
    private readonly Fish Fish;
    [SerializeField]
    private readonly Ship Ship;
    [SerializeField]
    private readonly Tool Tool;
}
