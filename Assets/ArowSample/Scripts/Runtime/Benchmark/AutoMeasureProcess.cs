using System;
using System.Collections;
using System.IO;
using ArowLibrary.ArowDefine.SchemaWrapper;
using ArowMain.Runtime;
using UnityEngine;
using UnityEngine.Networking;

namespace ArowSample.Scripts.Runtime
{
/// <summary>
/// 処理速度自動計測スクリプト
/// </summary>
public class AutoMeasureProcess : MonoBehaviour
{

    private CreateRuntimeRoadBuilder RoadBuildComponent
    {
        get
        {
            var x = gameObject.GetComponent<CreateRuntimeRoadBuilder>();

            if (x == null)
            {
                x = gameObject.AddComponent<CreateRuntimeRoadBuilder>();
            }

            return x;
        }
    }

    private CreateRuntimeBuildingBuilder BuildingBuildComponent
    {
        get
        {
            var x = gameObject.GetComponent<CreateRuntimeBuildingBuilder>();

            if (x == null)
            {
                x = gameObject.AddComponent<CreateRuntimeBuildingBuilder>();
            }

            return x;
        }
    }

    private CreateRuntimeGroundBuilder GroundBuildComponent
    {
        get
        {
            var x = gameObject.GetComponent<CreateRuntimeGroundBuilder>();

            if (x == null)
            {
                x = gameObject.AddComponent<CreateRuntimeGroundBuilder>();
            }

            return x;
        }
    }

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this);
        StartCoroutine(MeasureProcess());
    }

    IEnumerator MeasureProcess()
    {
        ArowSceneManager.ChangeScene(ArowSceneManager.WalkSceneName);
        yield return new WaitForSeconds(3.0f);
        Debug.Log("道の自動生成 時間計測開始");
        MeasureProcessTime.Setup();
        DownloadMapData(ArowSceneManager.FileName, (byte[] bytes) =>
        {
            RoadBuildComponent.CreateRoads(ArowMapObjectModel.LoadByData(bytes), OSMDefine.CREATE_WAY.HIGH_WAY);
        });
        yield return new WaitForSeconds(10.0f);
        ArowSceneManager.ChangeScene(ArowSceneManager.WalkSceneName);
        yield return new WaitForSeconds(3.0f);
        Debug.Log("建物の自動生成 時間計測開始");
        MeasureProcessTime.Setup();
        DownloadMapData(ArowSceneManager.FileName, (byte[] bytes) =>
        {
            BuildingBuildComponent.CreateBuildings(ArowMapObjectModel.LoadByData(bytes));
        });
        yield return new WaitForSeconds(10.0f);
        ArowSceneManager.ChangeScene(ArowSceneManager.WalkSceneName);
        yield return new WaitForSeconds(3.0f);
        Debug.Log("地形の自動生成 時間計測開始");
        MeasureProcessTime.Setup();
        DownloadMapData(ArowSceneManager.FileName, (byte[] bytes) =>
        {
            GroundBuildComponent.CreateGround(ArowMapObjectModel.LoadByData(bytes));
        });
        yield return new WaitForSeconds(10.0f);
        ArowSceneManager.ChangeScene(ArowSceneManager.WalkSceneName);
        yield return new WaitForSeconds(3.0f);
        Debug.Log("川の自動生成 時間計測開始");
        MeasureProcessTime.Setup();
        DownloadMapData(ArowSceneManager.FileName, (byte[] bytes) =>
        {
            RoadBuildComponent.CreateRoads(ArowMapObjectModel.LoadByData(bytes), OSMDefine.CREATE_WAY.WATER_WAY);
        });
        yield return new WaitForSeconds(10.0f);
        ArowSceneManager.ChangeScene(ArowSceneManager.WalkSceneName);
        yield return new WaitForSeconds(3.0f);
        Debug.Log("水域の自動生成 時間計測開始");
        MeasureProcessTime.Setup();
        DownloadMapData(ArowSceneManager.FileName, (byte[] bytes) =>
        {
            BuildingBuildComponent.CreateWater(ArowMapObjectModel.LoadByData(bytes));
        });
        MeasureProcessTime.Output();
        yield return null;
        Debug.Log("Prefab生成 時間計測開始");
        MeasureProcessTime.Setup();
        DownloadMapData(ArowSceneManager.FileName, (byte[] bytes) =>
        {
            var configList = Resources.Load<PrefabConfigList>("Demo/PrefabConfigList_demo");
            BuildingBuildComponent.CreatePrefabBuildings(ArowMapObjectModel.LoadByData(bytes), configList);
        });
        MeasureProcessTime.Output();
        yield return null;
    }
    private static void DownloadMapData(string filename, Action<byte[]> loadDataCallback)
    {
        var dirPath = Path.Combine(Application.temporaryCachePath, "arow_map");
        var filePath = Path.Combine(dirPath, filename);

        if (File.Exists(filePath))
        {
            loadDataCallback(File.ReadAllBytes(filePath));
        }
        else
        {
            var unityWebRequest = UnityWebRequest.Get(ArowSampleGame.SampleScripts.ArowURLDefine.DEFAULT_SERVER_URL + filename);
            RequestManager.SetWebRequest(unityWebRequest, (www) =>
            {
                if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.LogError(www.error);
                }
                else
                {
                    var data = www.downloadHandler.data;

                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }

                    File.WriteAllBytes(filePath, data);
                    loadDataCallback(data);
                }
            });
        }
    }
}

}
