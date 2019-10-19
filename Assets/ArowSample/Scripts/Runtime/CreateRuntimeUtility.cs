using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArowLibrary.ArowDefine.SchemaWrapper;
using ArowMain.Runtime;
using ArowMain.Runtime.CreateModelScripts;
using ArowSampleGame.SampleScripts;
using UnityEngine;

namespace ArowSample.Scripts.Runtime
{
public class CreateRuntimeUtility : MonoBehaviour
{
    // 仮想グリッドのサイズ
    private static readonly Vector2Int VirtualGridSize = new Vector2Int(8, 8);
    // 仮想グリッドの領域情報
    private Bounds[,] _virtualGridBounds = null;

    // 1Fの間にGameObject生成処理にかける時間
    private const float ObjectCreationProcessingTime = 1f / 120;

    private GameObject ParentObj;

    private NodeMapHolder nodeMapHolder;

    public NodeMapHolder CreateRoads(ArowMapObjectModel arowMapObjectModel, OSMDefine.CREATE_WAY key)
    {
        CreateConfigRoadMap config = ScriptableObject.CreateInstance<CreateConfigRoadMap>();
        config.RoadWidth = 6.0f;
        config.IsExtractWithTaskAsync = true;
        var nodeMapHolderParentInfo = GetOrCreateParentInfoFromArowMapObjectModel(arowMapObjectModel);
        nodeMapHolder = new NodeMapHolder();
        nodeMapHolder.Initialize(arowMapObjectModel.RoadDataModels, nodeMapHolderParentInfo.WorldCenter, nodeMapHolderParentInfo.WorldScale, key);
        // RoadのMesh生成に必要な情報(MeshSet)の抽出を行う.
        var roadMapMeshSetCreator = new RoadMapCreator
        .Builder(nodeMapHolder.nodeMap)
        .SetConfig(config)
        .SetOnCompletedCreateMeshCallBack((list) =>
        {
            // Meshの分割に必要な仮想グリッドの領域情報を生成
            _virtualGridBounds = CreateVirtualGridBounds(arowMapObjectModel, nodeMapHolderParentInfo, VirtualGridSize.x, VirtualGridSize.y);
            GameObject parent_obj = GetOrCreateParentInfoFromArowMapObjectModel(arowMapObjectModel).gameObject;
            // 実際のゲームオブジェクト生成を行う.
            StartCoroutine(CreateRoadGameObjects(list, parent_obj.transform, key));
            // 処理時間計測停止はタスク内で行う
            MeasureProcessTime.Stop();
        }, TaskScheduler.FromCurrentSynchronizationContext());
        // 処理時間計測開始（停止は「SetOnCompletedCreateMeshCallBack」で行われるはず
        MeasureProcessTime.Start(key == OSMDefine.CREATE_WAY.HIGH_WAY ? MeasureProcessTime.Key.Road : MeasureProcessTime.Key.WaterWay, "CreateRoads");
        roadMapMeshSetCreator.Build();
        return nodeMapHolder;
    }

    public static Vector2Int GetCenterPosition(ArowMapObjectModel arowMapObjectModel)
    {
        var mapRect = MapRectInt.FromDictionary(arowMapObjectModel.InfoList.FullInfoList);
        // 単純に中央値を出そうとするとオーバーフローするので注意
        return MapUtility.CalculateWorldCenter(mapRect.East, mapRect.North, mapRect.West, mapRect.South);
    }

    public static ParentInfo GetOrCreateParentInfoFromArowMapObjectModel(ArowMapObjectModel arowMapObjectModel)
    {
        return ParentInfo.GetOrCreateParentInfo(
                   arowMapObjectModel.GetHashCode().ToString(),
                   GetCenterPosition(arowMapObjectModel),
                   MapUtility.WorldScale);
    }

    private static Bounds[,] CreateVirtualGridBounds(ArowMapObjectModel arowMapObjectModel, ParentInfo parentInfo, int columns, int rows)
    {
        MeasureProcessTime.Start(MeasureProcessTime.Key.Unknown, "CreateVirtualGridBounds");

        if (arowMapObjectModel == null)
        {
            Debug.LogWarning("The argument of arowMapObjectModel is Null. It must be non-null.");
            return null;
        }

        var virtualGridBounds = new Bounds[columns, rows];
        var mapRect = MapRectInt.FromDictionary(arowMapObjectModel.InfoList.FullInfoList);
        var upperPosition = new Vector2(mapRect.East, mapRect.North);
        var lowerPosition = new Vector2(mapRect.West, mapRect.South);
        // グリッド化するエリアの全体幅を求める.
        var gridRegion = new Vector3(
            Math.Abs(upperPosition.x - lowerPosition.x) * parentInfo.WorldScale.x,
            0,
            Math.Abs(upperPosition.y - lowerPosition.y) * parentInfo.WorldScale.y);
        // 分割された1グリッドあたりのサイズを求める.
        var boundsSize = new Vector3(gridRegion.x / columns, 10.0f, gridRegion.z / rows);
        // グリッド同士の中心点の距離を計算するため, Offset値を算出する.
        var offsetX = boundsSize.x / 2.0f;
        var offsetZ = boundsSize.z / 2.0f;
        // 仮想グリッドの基準点座標.
        var basePoint = new Vector3(
            (lowerPosition.x - parentInfo.WorldCenter.x) * parentInfo.WorldScale.x,
            0,
            (lowerPosition.y - parentInfo.WorldCenter.y) * parentInfo.WorldScale.y
        );

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                var centerPosition = new Vector3(
                    basePoint.x + offsetX * (2 * i + 1),
                    0,
                    basePoint.z + offsetZ * (2 * j + 1)
                );
                var bounds = new Bounds(centerPosition, boundsSize);
                virtualGridBounds[i, j] = bounds;
            }
        }

        return virtualGridBounds;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_virtualGridBounds != null)
        {
            // 仮想グリッドをGizmoとして表示する
            foreach (var bounds in _virtualGridBounds)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
        }
    }
#endif

    private List<CreateRoadMeshScripts.MeshSet> GetMeshSets(
        Bounds[,] virtualGridBounds, Vector2Int targetGridIndex, CreateRoadMeshScripts.MeshSet[] meshSets)
    {
        // Listリサイズに伴う再割当てでのパフォーマンス低下を避けるため、
        // ListのCapacityを想定される最大サイズに設定する.
        var includedMeshSets = new List<CreateRoadMeshScripts.MeshSet>(meshSets.Length);
        var virtualGridBound = virtualGridBounds[targetGridIndex.x, targetGridIndex.y];

        foreach (var meshSet in meshSets)
        {
            // Mesh内各頂点がグリッド内に所属するかを判定し、振り分ける
            foreach (var vertex in meshSet.VerticesList)
            {
                if (virtualGridBound.Contains(vertex))
                {
                    includedMeshSets.Add(meshSet);
                    break;
                }
            }
        }

        return includedMeshSets;
    }

    /// <summary>
    /// Mesh内各頂点がグリッド内に所属するかを判定
    /// </summary>
    /// <param name="virtualGridBounds"></param>
    /// <param name="targetGridIndex"></param>
    /// <param name="meshSet"></param>
    /// <returns></returns>
    private bool IsInsideVirtualGridBound(Bounds[,] virtualGridBounds, Vector2Int targetGridIndex, CreateRoadMeshScripts.MeshSet meshSet)
    {
        var virtualGridBound = virtualGridBounds[targetGridIndex.x, targetGridIndex.y];

        foreach (var vertex in meshSet.VerticesList)
        {
            if (virtualGridBound.Contains(vertex))
            {
                return true;
            }
        }

        return false;
    }


    private IEnumerator CreateRoadGameObjects(List<Tuple<MeshRelationInfo, CreateRoadMeshScripts.MeshSet>> list, Transform parent, OSMDefine.CREATE_WAY kind)
    {
        var straightRoadObjectCount = 0;
        var curveRoadObjectCount = 0;
        var areaRoadObjectCount = 0;
        var startTime = Time.realtimeSinceStartup;

        foreach (var tuple in list)
        {
            var roadType = tuple.Item1;
            var meshSet = tuple.Item2;

            switch (roadType.RoadType)
            {
                case RoadMapCreator.RoadType.Straight:
                    CreateStraightRoadGameObject("Straight_" + roadType.Id, meshSet, parent, kind);
                    straightRoadObjectCount++;
                    break;

                case RoadMapCreator.RoadType.Curve:
                    CreateCurveRoadGameObject("Curve_" +  roadType.Id, meshSet, parent, kind);
                    curveRoadObjectCount++;
                    break;

                case RoadMapCreator.RoadType.Area:
                    CreateAreaGameObject("Cross_" +  roadType.Id, meshSet, parent, kind);
                    areaRoadObjectCount++;
                    break;
            }

            if (Time.realtimeSinceStartup - startTime >= ObjectCreationProcessingTime)
            {
                yield return null;
                startTime = Time.realtimeSinceStartup;
            }
        }
    }

    private void CreateStraightRoadGameObject(string objectName, CreateRoadMeshScripts.MeshSet meshSet, Transform parent, OSMDefine.CREATE_WAY kind)
    {
        // 区切ったグリッドのうち、全グリッド内にある道路のMeshSetを取得する.
        for (int i = 0; i < VirtualGridSize.x; i++)
        {
            for (int j = 0; j < VirtualGridSize.y; j++)
            {
                var gridPosition = new Vector2Int(i, j);

                if (IsInsideVirtualGridBound(_virtualGridBounds, gridPosition, meshSet))
                {
                    string materialName = null;

                    switch (kind)
                    {
                        default:
                            Debug.Assert(false);
                            break;

                        case OSMDefine.CREATE_WAY.HIGH_WAY:
                            materialName = "Roads/StraightRoad";
                            break;

                        case OSMDefine.CREATE_WAY.WATER_WAY:
                            materialName = "Rivers/WaterMaterial";
                            break;
                    }

                    CreateGameObject(meshSet, objectName, materialName, parent);
                }
            }
        }
    }

    private void CreateCurveRoadGameObject(string objectName, CreateRoadMeshScripts.MeshSet meshSet, Transform parent, OSMDefine.CREATE_WAY kind)
    {
        string materialName = null;

        switch (kind)
        {
            default:
                Debug.Assert(false);
                break;

            case OSMDefine.CREATE_WAY.HIGH_WAY:
                materialName = "Roads/StraightRoad";
                break;

            case OSMDefine.CREATE_WAY.WATER_WAY:
                materialName = "Rivers/WaterMaterial";
                break;
        }

        CreateGameObject(meshSet, objectName, materialName, parent);
    }

    private void CreateAreaGameObject(string objectName, CreateRoadMeshScripts.MeshSet meshSet, Transform parent, OSMDefine.CREATE_WAY kind)
    {
        string materialName = null;

        switch (kind)
        {
            default:
                Debug.Assert(false);
                break;

            case OSMDefine.CREATE_WAY.HIGH_WAY:
                materialName = "Roads/CrossRoad";
                break;

            case OSMDefine.CREATE_WAY.WATER_WAY:
                materialName = "Rivers/WaterMaterial";
                break;
        }

        CreateGameObject(meshSet, objectName, materialName, parent);
    }

    private void CreateGameObject(
        CreateRoadMeshScripts.MeshSet meshSet, string objectName, string materialName, Transform parent
    )
    {
        var obj = new GameObject(objectName);
        var meshFilter = obj.AddComponent<MeshFilter>();
        var mesh = meshSet.CreateMesh();
        meshFilter.sharedMesh = mesh;
        var meshRenderer = obj.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = Resources.Load<Material>(materialName);
        var meshCollider = obj.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        obj.transform.SetParent(parent);
    }

}
}
