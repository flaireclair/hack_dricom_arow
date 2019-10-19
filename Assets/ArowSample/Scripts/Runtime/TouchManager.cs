using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ArowMain.Runtime;
using ArowMain.Runtime.CreateModelScripts;
using ArowLibrary.ArowDefine.SchemaWrapper;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace ArowSample.Scripts.Runtime
{
public class TouchManager : MonoBehaviour
{

    private Vector3 _startVector3 = Vector3.zero;
    private int _cameraControlType = 0;
    private UnityChanAnimator _unityChanAnimator;
    private FollowCamera _followCamera;

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


    void Start()
    {
        _unityChanAnimator = GameObject.Find("Walkman_unitychan").GetComponent<UnityChanAnimator>();
        _followCamera = Camera.main.GetComponent<FollowCamera>();
    }

    void Update()
    {
        Vector3 movedVector3 = GetMovedVector3();

        if (_cameraControlType == 0)
        {
            _unityChanAnimator.InputVector3 = movedVector3;
            _followCamera.InputVector3 = Vector3.zero;
        }
        else
        {
            _unityChanAnimator.InputVector3 = Vector3.zero;
            _followCamera.InputVector3 = movedVector3;
        }
    }

    private Vector3 GetMovedVector3()
    {
        TouchInfo touchInfo = TouchUtils.GetTouch();

        if (touchInfo.Equals(TouchInfo.None))
        {
            _startVector3 = Vector3.zero;
        }
        else if (touchInfo.Equals(TouchInfo.Began))
        {
            _startVector3 = TouchUtils.GetTouchPosition();
        }
        else if (touchInfo.Equals(TouchInfo.Moved))
        {
            return TouchUtils.GetTouchPosition() - _startVector3;
        }

        return Vector3.zero;
    }

    void OnGUI()
    {
        const int HEIGHT = 50;
        var row = 1;
        {
            var x = 0;
            var y = HEIGHT * (row - 1);
            var width = Screen.width / 2;

            if (GUI.Button(new Rect(x, y, width, HEIGHT), (_cameraControlType == 0) ? "Move" : "Camera"))
            {
                _cameraControlType = (1 - _cameraControlType);
            }

            x = x + width;

            if (GUI.Button(new Rect(x, y, width, HEIGHT), "Camera Reset"))
            {
                _followCamera.CameraReset();
            }

            row++;
        }
        {
            var x = 0;
            var y = 50;
            var width = Screen.width / 7;
            var height = 50;

            if (GUI.Button(new Rect(x, y, width, HEIGHT), "スタートシーンへ"))
            {
                ArowSceneManager.ChangeScene(ArowSceneManager.StartSceneName);
            }

            if (ArowSceneManager.FileName.EndsWith("arowmap"))
            {
                // 地図データ のダウンロード
                x += width;

                if (GUI.Button(new Rect(x, y, width, HEIGHT), "道の自動生成"))
                {
                    DownloadMapData(ArowSceneManager.FileName, (byte[] bytes) =>
                    {
                        RoadBuildComponent.CreateRoads(ArowMapObjectModel.LoadByData(bytes), OSMDefine.CREATE_WAY.HIGH_WAY);
                    });
                }

                x += width;

                if (GUI.Button(new Rect(x, y, width, HEIGHT), "建物の自動生成"))
                {
                    DownloadMapData(ArowSceneManager.FileName, (byte[] bytes) =>
                    {
                        BuildingBuildComponent.CreateBuildings(ArowMapObjectModel.LoadByData(bytes));
                    });
                }

                x += width;

                if (GUI.Button(new Rect(x, y, width, HEIGHT), "地形の自動生成"))
                {
                    DownloadMapData(ArowSceneManager.FileName, (byte[] bytes) =>
                    {
                        GroundBuildComponent.CreateGround(ArowMapObjectModel.LoadByData(bytes));
                    });
                }

                x += width;

                if (GUI.Button(new Rect(x, y, width, height), "川の自動生成"))
                {
                    DownloadMapData(ArowSceneManager.FileName, (byte[] bytes) =>
                    {
                        RoadBuildComponent.CreateRoads(ArowMapObjectModel.LoadByData(bytes), OSMDefine.CREATE_WAY.WATER_WAY);
                    });
                }

                x += width;

                if (GUI.Button(new Rect(x, y, width, height), "湖の自動生成"))
                {
                    DownloadMapData(ArowSceneManager.FileName, (byte[] bytes) =>
                    {
                        BuildingBuildComponent.CreateWater(ArowMapObjectModel.LoadByData(bytes));
                    });
                }

                x += width;

                if (GUI.Button(new Rect(x, y, width, height), "prefab置換"))
                {
                    DownloadMapData(ArowSceneManager.FileName, (byte[] bytes) =>
                    {
                        var configList = Resources.Load<PrefabConfigList>("Demo/PrefabConfigList_demo");
                        BuildingBuildComponent.CreatePrefabBuildings(ArowMapObjectModel.LoadByData(bytes), configList);
                    });
                }
            }

            x += width;

            if (GUI.Button(new Rect(x, y, width, HEIGHT), "キャッシュクリア"))
            {
                DeleteMapData();
            }

            row++;
        }
        {
            var x = 0;
            var y = HEIGHT * (row - 1);
            var width = Screen.width / 2;

            if (GUI.Button(new Rect(x, y, width, HEIGHT), "建物の自動生成\n(インテリアマッピング)"))
            {
                DownloadMapData(ArowSceneManager.FileName, (byte[] bytes) =>
                {
                    BuildingBuildComponent.CreateBuildingsByInteriorMapping(ArowMapObjectModel.LoadByData(bytes));
                });
            }

            x += width;
            width /= 4;

            if (GUI.Button(new Rect(x, y, width, HEIGHT), "建物の自動生成\n(9スライス)"))
            {
                DownloadMapData(ArowSceneManager.FileName, (byte[] bytes) =>
                {
                    BuildingBuildComponent.CreateBuildingsByNineSlice(ArowMapObjectModel.LoadByData(bytes));
                });
            }

            row++;
        }
        {
            var width = Screen.width - 60 * 2;
            var height = Screen.height - 60;

            if (GUI.Button(new Rect(width, height, 50, 50), "計測"))
            {
                GameObject t = new GameObject();
                t.AddComponent<AutoMeasureProcess>();
            }
        }
    }

    private static void DeleteMapData()
    {
        var dirPath = Path.Combine(Application.temporaryCachePath, "arow_map");

        if (Directory.Exists(dirPath))
        {
            Directory.Delete(dirPath, true);
        }
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
