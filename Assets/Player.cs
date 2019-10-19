using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static int money;
    public static Object.Fish[] fish;
    public static Object.Ship[] ships;
    public static Object.Tool[] tools;
    Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = new Player();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
