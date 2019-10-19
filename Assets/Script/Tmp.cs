using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tmp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("CanFishing");
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator CanFishing()
    {
        float waitTime = Random.Range(1.0f, 30.0f);
        Debug.Log(string.Format("Wait : {0}", waitTime));
        yield return new WaitForSeconds(waitTime);
        float fishingTime = Time.deltaTime;
        yield return new WaitWhile(() => AppUtil.GetTouch() == TouchInfo.Canceled);
        if (Time.deltaTime - fishingTime > 2.0f) Debug.Log(true);
        yield return null;
    }
}
