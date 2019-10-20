using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookManagerLR : MonoBehaviour
{
    public GameObject Base;
    private static Transform nowPos;
    private static float nowX = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TransformL()
    {
        StartCoroutine("MoveBase", "L");
    }

    public void TransformR()
    {
        StartCoroutine("MoveBase", "R");
    }

    IEnumerator MoveBase(string wl)
    {
        int distance = wl == "L" ? -1 : 1;
        yield return new WaitUntil(() => { Base.transform.position += new Vector3(distance * 0.1f, 0, 0); nowX += distance * 0.1f; Debug.Log(nowX); return nowX*10 * distance >= 100f; });
        yield break;
    }
}
