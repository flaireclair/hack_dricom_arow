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

namespace ArowSample.Scripts.Runtime
{
public class GpsPlayerStart : MonoBehaviour
{
    private LocationManager _locationManager = null;
    [SerializeField]
    private Text text;

    [SerializeField]
    private GameObject unityChan;
    private ParentInfo parentInfo;

    private const string AROW_FILE_NAME = "meguro.arowmap";

    void Start()
    {
        _locationManager = GetComponent<LocationManager>();
        StartCoroutine(LoadMap());
    }

    IEnumerator LoadMap()
    {
        yield return new WaitForSecondsRealtime(1f);
        var filePath = Path.Combine(Application.streamingAssetsPath, AROW_FILE_NAME);
#if !UNITY_EDITOR && UNITY_ANDROID
        var request = UnityEngine.Networking.UnityWebRequest.Get(filePath);
        request.SendWebRequest();

        while (!request.downloadHandler.isDone)
        {
            yield return null;
        }

        var mapDataBytes = request.downloadHandler.data;
#else
        var mapDataBytes = File.ReadAllBytes(filePath);
#endif
        var arowMapObjectModel = ArowMapObjectModel.LoadByData(mapDataBytes);
        CreateConfigGroundMap config = ScriptableObject.CreateInstance<CreateConfigGroundMap>();
        config.RoadDataModels = arowMapObjectModel.RoadDataModels;
        config.HeightScale = 2f;
        // ArowSample 利用
        parentInfo = CreateRuntimeUtility.GetOrCreateParentInfoFromArowMapObjectModel(arowMapObjectModel);
        GroundMapCreator.Builder builder =
            new GroundMapCreator.Builder(arowMapObjectModel,
                                         parentInfo.WorldCenter,
                                         parentInfo.WorldScale)
        .SetParentTransform(parentInfo.transform)
        .SetConfig(config)
        ;
        builder.Build();
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
}
