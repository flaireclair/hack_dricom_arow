using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static int money = 0;
    public static List<FishEntity> fish = new List<FishEntity>();
    public static List<ShipEntity> ships = new List<ShipEntity>();
    public static List<ToolEntity> tools = new List<ToolEntity>();
    Player player;
    // Start is called before the first frame update
    void Awake()
    {
        player = new Player();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
