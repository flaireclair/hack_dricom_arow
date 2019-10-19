using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ArowLibrary.ArowDefine.SchemaWrapper;
using ArowMain.Runtime;
using ArowMain.Runtime.CreateModelScripts;
using ArowSample.Scripts.Runtime;
using UnityEngine;


namespace ArowSampleGame.SampleScripts
{
/// <summary>
/// SampleGame内で使用する AROW Map の取り扱い関数群.
/// </summary>
public class ArowDemoMain : MonoBehaviour
{

    public enum MAP_CREATE
    {
        ROAD,
        BUILDING,
        GROUND,
    }

    // AROWMapの置き場所（StreamingAssetsから直接読み込む）
    private readonly string AROW_FILE_NAME = "shibuya.arowmap";

    // 各地図に関連したオブジェクトの「親」の名前
    private readonly string BUILDING_OBJ_PARENT_NAME = "building_parent";
    private readonly string ROAD_OBJ_PARENT_NAME = "road_parent";
    private readonly string GROUND_OBJ_PARENT_NAME = "ground_parent";

    private Dictionary<MAP_CREATE, bool> isEndOfFunction = new Dictionary<MAP_CREATE, bool>();

    private readonly Vector2 ERROR_MSG_WINDOW_SIZE = new Vector2(500, 150);

    private const int ERROR_MSG_FONT_SIZE = 23;

    public Dictionary<MAP_CREATE, bool> IsEnd
    {
        get
        {
            return isEndOfFunction;
        }
    }

    // AROWMAPのバイナリ
    private byte[] mapDataBytes;

    // AROWMAPのバイナリを扱いやすくしたデータ
    private ArowMapObjectModel arowMapObjectModel = null;

    // 保持用変数
    private Vector2Int worldCenter = Vector2Int.zero;
    public float OrthographicSize
    {
        get;
        private set;
    }

    private void Start()
    {
    }

    /// <summary>
    /// Get the world center.
    /// </summary>
    /// <returns>The world center.</returns>
    public Vector2Int GetWorldCenter()
    {
        return worldCenter;
    }


    // 初期化
    public void Initialize()
    {
        // AROWマップデータの読み込み
        var filePath = Path.Combine(Application.streamingAssetsPath, AROW_FILE_NAME);
#if !UNITY_EDITOR && UNITY_ANDROID
        var request = UnityEngine.Networking.UnityWebRequest.Get(filePath);
        request.SendWebRequest();

        while (!request.downloadHandler.isDone)
        {
        }

        mapDataBytes = request.downloadHandler.data;
#else
        mapDataBytes = File.ReadAllBytes(filePath);
#endif
        arowMapObjectModel = ArowMapObjectModel.LoadByData(mapDataBytes);
        var mapRect = MapRectInt.FromDictionary(arowMapObjectModel.InfoList.FullInfoList);
        worldCenter = MapUtility.CalculateWorldCenter(mapRect.East, mapRect.North, mapRect.West, mapRect.South);

        foreach (MAP_CREATE i in Enum.GetValues(typeof(MAP_CREATE)))
        {
            isEndOfFunction.Add(i, false);
        }

        OrthographicSize = (mapRect.North - mapRect.South) / 2f * MapUtility.WorldScale.x;
    }

    /// <summary>
    /// DEMO用のPoiObserver初期化関数
    /// </summary>
    public void InitializePoiObserver()
    {
        // Observer系セットアップ開始
        GameObject go = GameObject.Find("Walkman_unitychan_sample");
        PoiVisitorSample ps = go.AddComponent<PoiVisitorSample>();
        GameObject ob = new GameObject("PoiObserver");
        ArowPoiObserver poiObserver = ob.AddComponent<ArowPoiObserver>();
        //必要なパラメータ流し込み
        poiObserver.RegisterTarget(go, ps);
        poiObserver.SetWorldCenter(worldCenter);
        poiObserver.RegisterObservePoi(arowMapObjectModel, AROW_FILE_NAME);
        // デフォルトで、PoiObserverにが動作する様に「商業施設」「距離５０ｍ」を設定
        poiObserver.RegisterNoticeDistance(ArowLibrary.ArowDefine.POIDefine.CategoryTbl[ArowLibrary.ArowDefine.POIDefine.CATEGORY.COMMERCIAL_FACILITY].name, 50);
    }

    /// <summary>
    /// DEMO用のConfig関連の読み込み
    /// ※Demo用のConfigファイルなので、読み込み先は決め打ちしている
    /// </summary>
    /// <param name="use_landmark_config">true:LandmarkConfigを読み込む</param>
    /// <param name="use_poi_config">true:PoiConfigを読み込む</param>
    public void CreateBuildingFromConfigAsset(bool use_landmark_config, bool use_poi_config)
    {
#if UNITY_EDITOR

        if (!File.Exists("Assets/ArowSample/Resources/Demo/BuildingConfig.asset"))
        {
            DisplayDemoError("asset ファイルが無いためビル生成ができません。ArowSample メニューからセットアップを実行してください。");
            return;
        }

#endif
        CreateConfig configpublic = Resources.Load<CreateConfig>("Demo/BuildingConfig");

        if (use_landmark_config)
        {
#if UNITY_EDITOR

            if (!File.Exists("Assets/ArowSample/Resources/Demo/LandmarkPoiConfig_DemoSample.asset"))
            {
                DisplayDemoError("LandmarkPoiConfig ファイルが無いためビル生成ができません。ArowSample メニューからセットアップを実行してください。");
                return;
            }

#endif
            LandmarkPoiConfig configlandmark = Resources.Load<LandmarkPoiConfig>("Demo/LandmarkPoiConfig_DemoSample");
            configpublic.LandmarkConfig = configlandmark;
        }

        if (use_poi_config)
        {
#if UNITY_EDITOR

            if (!File.Exists("Assets/ArowSample/Resources/Demo/CategoryPoiConfig_DemoSample.asset"))
            {
                DisplayDemoError("CategoryPoiConfig ファイルが無いためビル生成ができません。ArowSample メニューからセットアップを実行してください。");
                return;
            }

#endif
            CategoryPoiConfig configpoi = Resources.Load<CategoryPoiConfig>("Demo/CategoryPoiConfig_DemoSample");
            configpublic.CategoryPoiConfig = configpoi;
        }

        CreateBuildings(configpublic);
    }

    //ディベロッパーへエラーを伝えやすい様にゲーム画面にもウインドウにてメッセージを表示する関数
    private void DisplayDemoError(string error_msg)
    {
        // 先にエラーログを出力
        Debug.LogError(error_msg);
        // ゲーム画面表示用のエラーメッセージウインドウを用意
        GameObject canvas = GameObject.Find("Canvas");
        GameObject ugui_error = Instantiate<GameObject>(Resources.Load<GameObject>("UI/ErrorImage"));
        ugui_error.transform.SetParent(canvas.transform);
        GameObject text_obj = ugui_error.transform.Find("Text").gameObject;
        UnityEngine.UI.Text t = text_obj.GetComponent<UnityEngine.UI.Text>();
        RectTransform text_rt = text_obj.transform as RectTransform;
        // ウインドウの位置変更
        RectTransform prefab_rt = ugui_error.transform as RectTransform;
        prefab_rt.localPosition = new Vector3(0, 0, 0);
        // ウインドウサイズ変更
        prefab_rt.sizeDelta = ERROR_MSG_WINDOW_SIZE;
        text_rt.sizeDelta = ERROR_MSG_WINDOW_SIZE;
        // ウインドウにメッセージ流し込み
        t.text = error_msg;
        t.fontSize = ERROR_MSG_FONT_SIZE;
    }

    public void CreateBuildingsAttachmentCollider()
    {
        // 建物などに関しての「設定ファイル」の作成
        CreateConfig config = ScriptableObject.CreateInstance<CreateConfig>();
        CreateRuntimeBuildingBuilder.SetupConfigForSampleSlice(config);
        CreateRuntimeBuildingBuilder.SetupFillGapGroundElement(config);
        config.SetupSharedMaterials = CreateRuntimeBuildingBuilder.GetFixTextureSuiteCallbackForSampleSlice(config);
        // 当たり判定
        config.BuildingColliderElement = CreateConfig.BuildingCollider.Mesh;
        CreateBuildings(config);
    }

    public void CreateBuildingsNonCollider()
    {
        // 建物などに関しての「設定ファイル」の作成
        CreateConfig config = ScriptableObject.CreateInstance<CreateConfig>();
        CreateRuntimeBuildingBuilder.SetupConfigForSampleSlice(config);
        CreateRuntimeBuildingBuilder.SetupFillGapGroundElement(config);
        config.SetupSharedMaterials = CreateRuntimeBuildingBuilder.GetFixTextureSuiteCallbackForSampleSlice(config);
        CreateBuildings(config);
    }

    // ビル生成
    private void CreateBuildings(CreateConfig config)
    {
        Debug.Assert(arowMapObjectModel != null);
        var p = GameObject.Find(BUILDING_OBJ_PARENT_NAME);

        if (p == null)
        {
            p = new GameObject(BUILDING_OBJ_PARENT_NAME);
        }

        // 「BuildingのMesh生成」に必要な設定を行う
        var buildingMeshCreator = new BuildingCreator
        .Builder(arowMapObjectModel.BuildingDataModels)
        .SetRoadDataModels(arowMapObjectModel.RoadDataModels)
        .SetWorldCenter(worldCenter)
        .SetWorldScale(MapUtility.WorldScale)
        .SetConfig(config)
        .IsExtractWithTaskAsync(true)
        .SetOnMeshBuildType(BuildingCreator.BUILD_TYPE.BUILDING)
        .SetOnMeshCreatedCallBack((BuildingDataModelWithMesh buildingDataWithMesh) =>
        {
            // Mesh生成が完了するたびに、そのmeshをGameObject化する
            CreateBuildingMeshScripts.CreateBuildingGameObject(buildingDataWithMesh, p.transform, worldCenter, MapUtility.WorldScale, config);
        })
        .SetOnCompletedCreateMeshCallBack((List<BuildingDataModelWithMesh> list) =>
        {
            // 全てのmeshを生成し終えたら、state切り替え
            isEndOfFunction[MAP_CREATE.BUILDING] = true;
        });
        // 「BuildingのMesh生成」の開始
        buildingMeshCreator.Build();
    }

    // 道の生成
    public NodeMapHolder CreateRoads()
    {
        Debug.Assert(arowMapObjectModel != null);
        GameObject obj = GameObject.Find(ROAD_OBJ_PARENT_NAME);

        if (obj == null)
        {
            obj = new GameObject(ROAD_OBJ_PARENT_NAME);
        }

        CreateConfigRoadMap config = ScriptableObject.CreateInstance<CreateConfigRoadMap>();
        config.RoadWidth = 6.0f;
        config.IsExtractWithTaskAsync = true;
        // 「道」のノード情報を設定
        NodeMapHolder nodeMapHolder = new NodeMapHolder();
        nodeMapHolder.Initialize(arowMapObjectModel.RoadDataModels, worldCenter, MapUtility.WorldScale, OSMDefine.CREATE_WAY.HIGH_WAY);
        // RoadのMesh生成に必要な情報(MeshSet)の抽出を行う.
        var roadMapMeshSetCreator = new RoadMapCreator
        .Builder(nodeMapHolder.nodeMap)
        .SetConfig(config)
        .SetOnCompletedCreateMeshCallBack((list) =>
        {
            StartCoroutine(CreateRoadGameObjects(list, obj.transform));
        }, TaskScheduler.FromCurrentSynchronizationContext());
        roadMapMeshSetCreator.Build();
        return nodeMapHolder;
    }
    /// <summary>
    /// 標高データを元に地面を生成。
    /// 生成前にconfigにて設定が可能
    /// </summary>
    /// <param name="groundHeightScale">Ground height scale.</param>
    /// <param name="isCreateRoadObjOrTex">If set to <c>true</c> is create road object or tex.</param>
    /// <param name="isToonLighting">If set to <c>true</c> is toon lighting.</param>
    /// <param name="isHeightColor">If set to <c>true</c> is height color.</param>
    public void CreateGround(float groundHeightScale, bool isCreateRoadObjOrTex, bool isToonLighting, bool isHeightColor)
    {
        Debug.Assert(arowMapObjectModel != null);
        GameObject ParentObj = GameObject.Find(GROUND_OBJ_PARENT_NAME);

        if (ParentObj == null)
        {
            ParentObj = new GameObject(GROUND_OBJ_PARENT_NAME);
        }

        CreateConfigGroundMap config = ScriptableObject.CreateInstance<CreateConfigGroundMap>();

        // 条件毎に個別設定も可能
        if (isCreateRoadObjOrTex)
        {
            // ３D地面用の「道」データが適用されたテクスチャの作成設定
            config.RoadDataModels = arowMapObjectModel.RoadDataModels;
        }

        if (isToonLighting)
        {
            // ライトの変更設定
            config.IsToonLighting = true;
        }

        if (isHeightColor)
        {
            // 標高の色分け設定
            config.IsVisibleColorHeight = true;
            config.MaxHeightContourLine = 30f * groundHeightScale;
            config.MinHeightContourLine = -10f;
        }

        // 重くない程度に解像度を高くする
        config.TexScale = Vector2.one * 2f;
        config.HeightScale = groundHeightScale;
        // 地面生成時の設定
        GroundMapCreator.Builder builder = new GroundMapCreator.Builder(arowMapObjectModel,
                worldCenter,
                MapUtility.WorldScale)
        .SetConfig(config)
        .SetParentTransform(ParentObj.transform);		// 生成時にまとめて設定も可能
        // 地面の生成開始
        builder.Build();
    }

    IEnumerator CreateRoadGameObjects(List<Tuple<MeshRelationInfo, CreateRoadMeshScripts.MeshSet>> list, Transform parentTransform)
    {
        // 実際のゲームオブジェクト生成を行う.
        Coroutine create = StartCoroutine(ArowSampleCreateRoad.CreateRoadGameObjects(list, parentTransform));
        yield return create;
        isEndOfFunction[(int)MAP_CREATE.ROAD] = true;
    }
}

}