using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSwimming : MonoBehaviour
{
    public float aaa, bbb, ccc;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Transform myTransform = this.transform;
        Vector3 worldPos = myTransform.position;

        do { Vector3 v = new Vector3(aaa,0,0);}
        while (worldPos.x >= bbb);
        Debug.Log("+x");

        do { transform.Rotate(new Vector3(0, 0, 10)); ccc += ccc; }
        while (ccc >= 18);

        do { Vector3 v = new Vector3(-aaa, 0, 0);}
        while (worldPos.x <= -bbb);
        Debug.Log("-x");
        
        do { transform.Rotate(new Vector3(0, 0, 10)); ccc += ccc; }
        while (ccc >= 18);
    }
}
