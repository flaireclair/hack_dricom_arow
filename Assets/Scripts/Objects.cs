using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objects : MonoBehaviour
{
    [SerializeField]
    private Fish _Fishes;
    [SerializeField]
    private Ship _Ships;
    [SerializeField]
    private Tool _Tools;
    public static Fish Fishes;
    public static Ship Ships;
    public static Tool Tools;

    private void Awake()
    {
        Fishes = _Fishes;
        Ships = _Ships;
        Tools = _Tools;
    }
}
