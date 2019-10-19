using System;
using System.Collections;
using System.Collections.Generic;
using ArowMain.Runtime;
using ArowMain.Runtime.CreateModelScripts;
using UnityEngine;

/// <summary>
/// 複数地図を読み込み、道を生成するサンプル用クラス
/// </summary>
public class CreateRoad
{
    // 1Fの間にGameObject生成処理にかける時間
    private const float ObjectCreationProcessingTime = 1f / 120;

    public static IEnumerator CreateRoadGameObjects(List<Tuple<MeshRelationInfo, CreateRoadMeshScripts.MeshSet>> list, Transform parentTransform, Dictionary<string, List<string>> belongObject = null)
    {
        var startTime = Time.realtimeSinceStartup;
        int objectCnt = 0;

        foreach (var tuple in list)
        {
            var relationInfo = tuple.Item1;
            var meshSet = tuple.Item2;
            bool isCreate = true;

            // 「道」がすでに生成されているのかチェック
            if (belongObject != null)
            {
                //
                foreach (var belong_list in belongObject)
                {
                    foreach (var create_road_id in belong_list.Value)
                    {
                        if (string.Equals(create_road_id, relationInfo.Id))
                        {
                            // すでに生成してある
                            isCreate = false;
                        }
                    }
                }
            }

            if (isCreate)
            {
                // 生成されていなければ作成
                var obj = CreateRoadGameObject(string.Format("{0}{1}_{2}", relationInfo.RoadType.ToString(), objectCnt, relationInfo.Id), meshSet);
                objectCnt++;
                obj.transform.SetParent(parentTransform);
            }

            if (Time.realtimeSinceStartup - startTime >= ObjectCreationProcessingTime)
            {
                yield return null;
                startTime = Time.realtimeSinceStartup;
            }
        }
    }

    private static GameObject CreateRoadGameObject(string objectName, CreateRoadMeshScripts.MeshSet meshSet)
    {
        var obj = new GameObject(objectName);
        var meshFilter = obj.AddComponent<MeshFilter>();
        var mesh = meshSet.CreateMesh();
        meshFilter.sharedMesh = mesh;
        var meshRenderer = obj.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = Resources.Load<Material>("Demo/StandardGray");
        var meshCollider = obj.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        return obj;
    }
}
