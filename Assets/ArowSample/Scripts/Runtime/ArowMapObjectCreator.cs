using System;
using System.Collections;
using System.Collections.Generic;
using ArowLibrary.ArowDefine.SchemaWrapper;
using ArowMain.Runtime;
using ArowMain.Runtime.CreateModelScripts;
using ArowSampleGame.SampleScripts;
using UnityEngine;


namespace ArowSampleGame.Runtime
{

// 「道」管理用
class RoadMapInfo
{
    public RoadMapCreator.Builder roadCreator;

    public bool isEndPrepare;		// リクエスト→mesh作成→「GameObjectの生成設定」まで終了
    public bool isEndCreateGameObject;		// GameObjectの生成まで完全に終了
    public bool isCreateRequest;
    public RoadMapInfo()
    {
        isEndCreateGameObject = false;
        isEndPrepare = false;
        isCreateRequest = false;
        roadCreator = null;
    }
}

// 「建物」管理用
class BuilgingInfo
{
    public BuildingCreator.Builder buildingCreator;

    public bool isEndPrepare;
    public bool isEndCreateGameObject;		// GameObjectの生成まで完全に終了
    public bool isCreateRequest;
    public BuilgingInfo()
    {
        isEndCreateGameObject = false;
        isEndPrepare = false;
        isCreateRequest = false;
        buildingCreator = null;
    }
}

// 「地面」管理用
class GroundMapInfo
{
    public GroundMapCreator.Builder groundCreator;

    public bool isEndPrepare;
    public bool isEndCreateGameObject;		// GameObjectの生成まで完全に終了
    public bool isCreateRequest;
    public GroundMapInfo()
    {
        isEndCreateGameObject = false;
        isEndPrepare = false;
        isCreateRequest = false;
        groundCreator = null;
    }
}

/// <summary>
/// 地図の動的ロード管理用クラス
/// </summary>
class DownloadMapInfo
{
    public bool isDownloadRequest;

    public RoadMapInfo roads;
    public BuilgingInfo building;
    public GroundMapInfo grounds;

    public int mapX;		// 経度　longitude
    public int mapY;		// 緯度　latitude

    private DownloadMapInfo() { }
    public DownloadMapInfo(int map_x, int map_y)
    {
        mapX = map_x;
        mapY = map_y;
        roads = new RoadMapInfo();
        building = new BuilgingInfo();
        grounds = new GroundMapInfo();
        isDownloadRequest = false;
    }
}

/// <summary>
/// 地図の制御を行い、動的にゲームオブジェクトを生成するクラス
/// </summary>
public class ArowMapObjectCreator : MonoBehaviour
{
    private ArowMapDownloader _mapDownloader;
    private Vector2Int worldCenter;

    private readonly string BUILDING_OBJ_PARENT_NAME = "building_parent";
    private readonly string ROAD_OBJ_PARENT_NAME = "Road_parent";
    private readonly string GROUND_OBJ_PARENT_NAME = "Ground_parent";
    private readonly string DICTIONARY_KEY_FORMAT = "{0}_{1}";
    private readonly string INT_TO_STRING_FORMAT = "0000000000";

    // 「道」生成管理用データ
    private Dictionary<string, DownloadMapInfo> roads = new Dictionary<string, DownloadMapInfo>();

    // 「道」生成後の制御用データ　緯度経度指定でGameObjectのアクティブOffなどのため
    private Dictionary<string, List<string>> belongObject = new Dictionary<string, List<string>>();

    /// <summary>
    /// 初期化関数
    /// 中心とする経度緯度を設定
    /// </summary>
    /// <param name="world_center">World center.</param>
    public void Initialize(Vector2Int world_center)
    {
        worldCenter = world_center;

        if (_mapDownloader == null)
        {
            _mapDownloader = new ArowMapDownloader();
        }
    }

    /// <summary>
    /// Creates the road.
    /// 「道」生成のリクエスト
    /// </summary>
    /// <param name="longitude">Longitude.</param>
    /// <param name="latitude">Latitude.</param>
    public void CreateRoad(int longitude, int latitude)
    {
        string key = GetDicKey(longitude, latitude);

        if (roads.ContainsKey(key))
        {
            if (roads[key].roads.isEndPrepare)
            {
                return;
            }
        }
        else
        {
            DownloadMapInfo temp = new DownloadMapInfo(longitude, latitude);
            roads.Add(key, temp);
        }

        if (roads[key].roads.isCreateRequest)
        {
            // TODO 何十回と呼ばれるようであれば調整
            //            Debug.LogFormat("CreateRoad[longitude({0}):latitude({1})] 二重リクエスト", longitude, latitude);
        }

        roads[key].roads.isCreateRequest = true;
    }

    /// <summary>
    /// Creates the building.
    /// 建物生成のリクエスト
    /// </summary>
    /// <param name="longitude">Longitude.</param>
    /// <param name="latitude">Latitude.</param>
    public void CreateBuilding(int longitude, int latitude)
    {
        string key = GetDicKey(longitude, latitude);

        if (roads.ContainsKey(key))
        {
            if (roads[key].building.isEndPrepare)
            {
                return;
            }
        }
        else
        {
            DownloadMapInfo temp = new DownloadMapInfo(longitude, latitude);
            roads.Add(key, temp);
        }

        if (roads[key].building.isCreateRequest)
        {
            Debug.LogFormat("CreateBuilding[longitude({0}):latitude({1})] 二重リクエスト", longitude, latitude);
        }

        roads[key].building.isCreateRequest = true;
    }

    /// <summary>
    /// Creates the ground.
    /// 地面生成のリクエスト
    /// </summary>
    /// <param name="longitude">Longitude.</param>
    /// <param name="latitude">Latitude.</param>
    public void CreateGround(int longitude, int latitude)
    {
        string key = GetDicKey(longitude, latitude);

        if (roads.ContainsKey(key))
        {
            if (roads[key].grounds.isEndPrepare)
            {
                return;
            }
        }
        else
        {
            DownloadMapInfo temp = new DownloadMapInfo(longitude, latitude);
            roads.Add(key, temp);
        }

        if (roads[key].grounds.isCreateRequest)
        {
            Debug.LogFormat("CreateGround[longitude({0}):latitude({1})] 二重リクエスト", longitude, latitude);
        }

        roads[key].grounds.isCreateRequest = true;
    }

    // 道の生成アップデート関数
    private void UpdataCreationRoad(string k)
    {
        // すでに生成済み
        if (roads[k].roads.isEndPrepare)
        {
            return;
        }

        // 作成しなくてよい
        if (!roads[k].roads.isCreateRequest)
        {
            return;
        }

        // arowmapダウンロード済みか
        if (!_mapDownloader.IsExistFile(roads[k].mapX, roads[k].mapY))
        {
            if (!roads[k].isDownloadRequest)
            {
                // ダウンロード開始
                StartCoroutine(_mapDownloader.ArowMapDownload(roads[k].mapX, roads[k].mapY));
                roads[k].isDownloadRequest = true;
            }

            return;
        }

        // 道生成を開始したか
        if (roads[k].roads.roadCreator == null)
        {
            CreateConfigRoadMap config = ScriptableObject.CreateInstance<CreateConfigRoadMap>();
            config.RoadWidth = 6.0f;
            config.IsExtractWithTaskAsync = true;
            // 道のメッシュ生成
            byte[] b = _mapDownloader.GetMap(roads[k].mapX, roads[k].mapY);
            var arowMapObjectModel = ArowMapObjectModel.LoadByData(b);
            // 「道」のノード情報を設定
            NodeMapHolder nodeMapHolder = new NodeMapHolder();
            nodeMapHolder.Initialize(arowMapObjectModel.RoadDataModels, worldCenter, MapUtility.WorldScale, OSMDefine.CREATE_WAY.HIGH_WAY);
            // RoadのMesh生成に必要な情報(MeshSet)の抽出を行う.
            roads[k].roads.roadCreator = new RoadMapCreator
            .Builder(nodeMapHolder.nodeMap)
            .SetConfig(config)
            .CreateMesh();
            return;
        }

        // 道メッシュが完成しているか
        if (!roads[k].roads.roadCreator.IsEndCreateMesh())
        {
            return;
        }

        GameObject obj;
        obj = GameObject.Find(ROAD_OBJ_PARENT_NAME);

        if (obj == null)
        {
            obj = new GameObject(ROAD_OBJ_PARENT_NAME);
        }

        // ゲームオブジェクト生成を行う.
        StartCoroutine(CreateRoadGameObjectCoroutine(k, roads[k].roads.roadCreator.GetMeshs(), obj.transform));
        // 色々と完了
        roads[k].roads.isCreateRequest = false;
        roads[k].roads.isEndPrepare = true;
    }

    //
    private IEnumerator CreateRoadGameObjectCoroutine(string key, List<Tuple<MeshRelationInfo, CreateRoadMeshScripts.MeshSet>> list, Transform parentTransform)
    {
        // 実際のゲームオブジェクト生成を行う.
        Coroutine c = StartCoroutine(ArowSampleCreateRoad.CreateRoadGameObjects(list, parentTransform, belongObject));
        yield return c;
        // 道オブジェクトの重複チェック用に、「ダウンロードした地図に含まれる道」を全て保存
        List<string> belong_ids = new List<string>();

        foreach (var id in roads[key].roads.roadCreator.GetMeshs())
        {
            belong_ids.Add(id.Item1.Id);
        }

        belongObject.Add(key, belong_ids);
        roads[key].roads.isEndCreateGameObject = true;
    }

    // 建物の生成アップデート関数
    void UpdataCreationBuilding(string k)
    {
        // すでに生成済み
        if (roads[k].building.isEndPrepare)
        {
            return;
        }

        // 作成しなくてよい
        if (!roads[k].building.isCreateRequest)
        {
            return;
        }

        // 地面生成後に作成させてみる
        if (!roads[k].grounds.isEndCreateGameObject)
        {
            return;
        }

        // arowmapダウンロード済みか
        if (!_mapDownloader.IsExistFile(roads[k].mapX, roads[k].mapY))
        {
            if (!roads[k].isDownloadRequest)
            {
                // ダウンロード開始
                StartCoroutine(_mapDownloader.ArowMapDownload(roads[k].mapX, roads[k].mapY));
                roads[k].isDownloadRequest = true;
            }

            return;
        }

        // 建物生成をしたか
        if (roads[k].building.buildingCreator == null)
        {
            var p = GameObject.Find(BUILDING_OBJ_PARENT_NAME);

            if (p == null)
            {
                p = new GameObject(BUILDING_OBJ_PARENT_NAME);
            }

            // 道生成
            byte[] b = _mapDownloader.GetMap(roads[k].mapX, roads[k].mapY);
            var arowMapObjectModel = ArowMapObjectModel.LoadByData(b);
            // 建物などに関しての「設定ファイル」の作成
            CreateConfig config = ScriptableObject.CreateInstance<CreateConfig>();
            config.BuildingColliderElement = CreateConfig.BuildingCollider.Mesh;
            config.BuildingWallMaterials.Add(Resources.Load<Material>("Buildings/Walls/wide_mat"));
            config.BuildingRoofMaterials.AddRange(Resources.LoadAll<Material>("Buildings/Roofs/FlatMaterials/"));

            switch (config.FillGapGroundElement)
            {
                case CreateConfig.FillGapGround.LiftupBuilding:
                    config.BuildingFillGapMaterials.AddRange(Resources.LoadAll<Material>("Buildings/FillGap/"));
                    break;
            }

            // 「BuildingのMesh生成」に必要な設定を行う
            roads[k].building.buildingCreator = new BuildingCreator
            .Builder(arowMapObjectModel.BuildingDataModels)
            .SetWorldCenter(worldCenter)
            .SetWorldScale(MapUtility.WorldScale)
            .SetConfig(config)
            .IsExtractWithTaskAsync(true)
            .SetOnMeshBuildType(BuildingCreator.BUILD_TYPE.BUILDING)
            .SetOnMeshCreatedCallBack((BuildingDataModelWithMesh buildingDataWithMesh) =>
            {
                // Mesh生成が完了するたびに、そのmeshをGameObject化する
                CreateBuildingMeshScripts.CreateBuildingGameObject(buildingDataWithMesh, p.transform, worldCenter, MapUtility.WorldScale, config);
            });
            // 「BuildingのMesh生成」の開始
            roads[k].building.buildingCreator.Build();
            return;
        }

        // 色々と完了
        roads[k].roads.isCreateRequest = false;
        roads[k].roads.isEndPrepare = true;
    }

    public void UpdataCreationGround(string k)
    {
        // すでに生成済み
        if (roads[k].grounds.isEndPrepare)
        {
            return;
        }

        // 作成しなくてよい
        if (!roads[k].grounds.isCreateRequest)
        {
            return;
        }

        // arowmapダウンロード済みか
        if (!_mapDownloader.IsExistFile(roads[k].mapX, roads[k].mapY))
        {
            if (!roads[k].isDownloadRequest)
            {
                // ダウンロード開始
                StartCoroutine(_mapDownloader.ArowMapDownload(roads[k].mapX, roads[k].mapY));
                roads[k].isDownloadRequest = true;
            }

            return;
        }

        // 建物生成をしたか
        if (roads[k].grounds.groundCreator == null)
        {
            GameObject ParentObj = GameObject.Find(GROUND_OBJ_PARENT_NAME);

            if (ParentObj == null)
            {
                ParentObj = new GameObject(GROUND_OBJ_PARENT_NAME);
            }

            // 道生成
            byte[] b = _mapDownloader.GetMap(roads[k].mapX, roads[k].mapY);
            var arowMapObjectModel = ArowMapObjectModel.LoadByData(b);
            CreateConfigGroundMap config = ScriptableObject.CreateInstance<CreateConfigGroundMap>();
            // ３D地面用の「道」データが適用されたテクスチャの作成設定
            config.RoadDataModels = arowMapObjectModel.RoadDataModels;
            config.TexScale = new Vector2(2f, 2f);
            // ライトの変更設定
            config.IsToonLighting = true;
            config.HeightScale = 8f;
            // 地面生成時の設定
            roads[k].grounds.groundCreator = new GroundMapCreator.Builder(arowMapObjectModel,
                    worldCenter,
                    MapUtility.WorldScale)
            .SetConfig(config)
            .SetParentTransform(ParentObj.transform);
            // 地面の生成開始
            StartCoroutine(CreateGroundGameObjectCoroutine(k));
        }
    }


    //
    private IEnumerator CreateGroundGameObjectCoroutine(string key)
    {
        // 実際のゲームオブジェクト生成を行う.
        Coroutine c = StartCoroutine(roads[key].grounds.groundCreator.CreateGroundGameObject());
        yield return c;
        roads[key].grounds.isCreateRequest = false;
        roads[key].grounds.isEndPrepare = true;
        roads[key].grounds.isEndCreateGameObject = true;
    }

    // 緯度経度からキーを作成
    private string GetDicKey(int i, int k)
    {
        return string.Format(DICTIONARY_KEY_FORMAT, i.ToString(INT_TO_STRING_FORMAT), k.ToString(INT_TO_STRING_FORMAT));
    }


    void Update()
    {
        foreach (var k in roads.Keys)
        {
            UpdataCreationGround(k);
            UpdataCreationRoad(k);
            UpdataCreationBuilding(k);
        }
    }

}
}