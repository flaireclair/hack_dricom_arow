using System;
using System.Collections;
using System.IO;
using ArowLibrary.ArowDefine.SchemaWrapper;
using ArowMain.Runtime;
using ArowMain.Runtime.CreateModelScripts;
using ArowMain.Runtime.GPS;
using ArowMain.Runtime.License;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    private LocationManager _locationManager = null;
    [SerializeField]
    private Text text;

    [SerializeField]
    private GameObject unityChan;
    private ParentInfo parentInfo;
    [SerializeField]
    private GameObject __null;
    public static GameObject _null;

    void Start()
    {
        _null = __null;
        StartCoroutine(InitializeCoroutine());
    }

    IEnumerator InitializeCoroutine()
    {
        yield return null;
        _locationManager = GetComponent<LocationManager>();
        parentInfo = ParentInfo.GetOrCreateParentInfo("parentInfo", new Vector2Int(1396900000, 359500000)/*(int)_locationManager.Latitude, (int)_locationManager.Longitude)*/, MapUtility.WorldScale);
        gameObject.AddComponent<ArowMapDynamicLoadManager>().Initialize(unityChan, new Vector2Int(1396900000, 359500000)); //(int)_locationManager.Latitude, (int)_locationManager.Longitude));
        yield return new WaitUntil(() => { try { if (_null.activeSelf) return true; } catch { } return false; });
        // 高い位置から地面へ Ray を飛ばす。
        var rayOriginHeight = 1000f;
        // 取得した経度緯度を AROW の経度緯度に合わせる。
        var rate = 10000000;
        var origin = new Vector3(
            (_locationManager.Longitude * rate - parentInfo.WorldCenter.x)
            * parentInfo.WorldScale.x,
            rayOriginHeight,
            (_locationManager.Latitude * rate - parentInfo.WorldCenter.y)
            * parentInfo.WorldScale.y
        );
        origin = new Vector3(0, rayOriginHeight, 0);

        Debug.Log(origin.ToString());
        RaycastHit hitInfo;

        if (Physics.Raycast(origin, Vector3.down, out hitInfo))
        {
            // 地面にぶつかったら ユニティちゃんを移動させる。
            unityChan.transform.position = hitInfo.point;
            Debug.Log(unityChan.transform.position.ToString());
            Debug.Log(hitInfo.point.ToString());
        }
        yield break;
    }

    void Update()
    {
#if UNITY_EDITOR
        text.text = "UnityEditorでは確認できません。";
#else

        if (_locationManager.Disabled)
        {
            text.text = "GPS機能を有効にすると確認できます。";
        }
        else
        {
            text.text = "位置情報が有効か: " + _locationManager.Started.ToString()
                        + "\n" + "lat: " + _locationManager.Latitude.ToString()
                        + "\n" + "lng: " + _locationManager.Longitude.ToString();
        }

#endif

        if (parentInfo == null)
        {
            return;
        }

        // 高い位置から地面へ Ray を飛ばす。
        var rayOriginHeight = 1000f;
        // 取得した経度緯度を AROW の経度緯度に合わせる。
        var rate = 10000000;
        var origin = new Vector3(
            (_locationManager.Longitude * rate - parentInfo.WorldCenter.x)
            * parentInfo.WorldScale.x,
            rayOriginHeight,
            (_locationManager.Latitude * rate - parentInfo.WorldCenter.y)
            * parentInfo.WorldScale.y
        );
        RaycastHit hitInfo;

        if (Physics.Raycast(origin, Vector3.down, out hitInfo))
        {
            // 地面にぶつかったら ユニティちゃんを移動させる。
            unityChan.transform.position = hitInfo.point;
        }
    }
}
