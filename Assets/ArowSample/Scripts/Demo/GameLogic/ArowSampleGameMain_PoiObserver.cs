using System;
using System.IO;
using ArowMain.Runtime;
using ArowSample.Scripts.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace ArowSampleGame.SampleScripts
{
public class ArowSampleGameMain_PoiObserver : MonoBehaviour
{
    enum STATE_AROW_MAP
    {
        NONE,
        DOWNLOAD_AROW_MAP,		// サーバーからarowmapデータをダウンロード.
        LOADING_AROW_MAP,		// ダウンロードしたデータの読み込み.
        CREATE_ROAD,			// 道の生成.
        CREATE_BUILDING,		// 建物の生成.
        SETTING_MAP,
        PLAYING_GAME,			//
    };
    /*
        // デバッグ用簡易仮データ
        string[] test_json = new string[]
        {
            "{\"category\": \"交通\",\"subcategory\": \"バス停\", \"name\": \"開発前\",\"latitude\": 42.29848876,\"longitude\": 143.31477485 }",
            "{\"category\": \"交通\",\"subcategory\": \"駅\", \"name\": \"渋谷\",\"latitude\": 35.6579965,\"longitude\": 139.7014889 }",
            "{\"category\": \"郵便局\",\"subcategory\": \"\", \"name\": \"渋谷桜丘郵便局\",\"latitude\": 35.654260,\"longitude\": 139.701174 }",
            "{\"category\": \"店舗\",\"subcategory\": \"\", \"name\": \"マルエツプチ\",\"latitude\": 35.654260,\"longitude\": 139.701174 }",
            "{\"category\": \"学校\",\"subcategory\": \"\", \"name\": \"青山製図専門学校\",\"latitude\": 35.654057,\"longitude\": 139.701783 }",
            "{\"category\": \"学校\",\"subcategory\": \"中学校\", \"name\": \"区立鉢山中学校\",\"latitude\": 35.653229,\"longitude\": 139.702961 }",
            "{\"category\": \"コンビニ\",\"subcategory\": \"ファミリーマート\", \"name\": \"ファミリーマート渋谷ガーデンフロント店\",\"latitude\": 35.654983,\"longitude\": 139.705067 }",
            "{\"category\": \"コンビニ\",\"subcategory\": \"セブン-イレブン\", \"name\": \"セブン-イレブン渋谷並木橋店\",\"latitude\": 35.653832,\"longitude\": 139.705817 }",

            "{\"category\": \"公共\",\"subcategory\": \"建物A\", \"name\": \"建物１\",\"latitude\": 35.654971,\"longitude\": 139.703509 }",
            "{\"category\": \"公共\",\"subcategory\": \"建物B\", \"name\": \"建物２\",\"latitude\": 35.654971,\"longitude\": 139.703509 }",
            "{\"category\": \"公共\",\"subcategory\": \"\", \"name\": \"建物３\",\"latitude\": 35.654971,\"longitude\": 139.703509 }",
            "{\"category\": \"コンビニ\",\"subcategory\": \"ファミリーマート\", \"name\": \"ファミリーマートA\",\"latitude\": 35.654971,\"longitude\": 139.703509 }",
        };

        string[] test2_add_json = new string[]
        {
            "{\"category\": \"公共\",\"subcategory\": \"建物C\", \"name\": \"建物４\",\"latitude\": 35.654972,\"longitude\": 139.70351 }",
            "{\"category\": \"公共\",\"subcategory\": \"建物D\", \"name\": \"建物５\",\"latitude\": 35.654972,\"longitude\": 139.70351 }",
            "{\"category\": \"公共\",\"subcategory\": \"\", \"name\": \"建物６\",\"latitude\": 35.654972,\"longitude\": 139.70351 }",
            "{\"category\": \"コンビニ\",\"subcategory\": \"ファミリーマート\", \"name\": \"ファミリーマートB\",\"latitude\": 35.654971,\"longitude\": 139.703509 }",
        };

    */
    private bool[] isStateEnd;



    private ArowDemoMain arowDemoMain;

    private STATE_AROW_MAP state = STATE_AROW_MAP.NONE;

    // Setting Inspector
    public Text StatusText;
    public GameObject WarkerObj;

    // Use this for initialization
    void Start()
    {
        var dirPath = Path.Combine(Application.temporaryCachePath, "arow_map");

        if (Directory.Exists(dirPath))
        {
            Directory.Delete(dirPath, true);
        }

        state = STATE_AROW_MAP.DOWNLOAD_AROW_MAP;
        isStateEnd = new bool[Enum.GetNames(typeof(STATE_AROW_MAP)).Length];

        for (int i = 0; i < isStateEnd.Length; i++)
        {
            isStateEnd[i] = false;
        }

        arowDemoMain = gameObject.AddComponent<ArowDemoMain>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case STATE_AROW_MAP.DOWNLOAD_AROW_MAP:
                // stremingAssetの中のデータを使用しているので、サーバーからのダウンロードは行なっていない
                state = STATE_AROW_MAP.LOADING_AROW_MAP;
                break;

            case STATE_AROW_MAP.LOADING_AROW_MAP:
                //
                arowDemoMain.Initialize();
                state = STATE_AROW_MAP.CREATE_ROAD;
                break;

            case STATE_AROW_MAP.CREATE_ROAD:
                //　３D地面生成
                arowDemoMain.CreateGround(5f, true, false, false);
                state = STATE_AROW_MAP.CREATE_BUILDING;
                break;

            case STATE_AROW_MAP.CREATE_BUILDING:
                // 建物の生成
                arowDemoMain.CreateBuildingsAttachmentCollider();
                // 3D地面の高さを強調すると地面の下にキャラクタが居ることになるので、高さだけ補正・・・
                GameObject.Find("Walkman_unitychan_sample").transform.localPosition = new Vector3(0f, 20f * 5f, 0f);
                state = STATE_AROW_MAP.SETTING_MAP;
                break;

            case STATE_AROW_MAP.SETTING_MAP:
                arowDemoMain.InitializePoiObserver();
                // Observer系セットアップ開始
                /*                GameObject go = GameObject.Find("Walkman_unitychan_sample");
                                PoiVisitorSample ps = go.AddComponent<PoiVisitorSample>();
                                GameObject ob = new GameObject("PoiObserver");
                                ArowPoiObserver poiObserver = ob.AddComponent<ArowPoiObserver>();
                                //必要なパラメータ流し込み
                                poiObserver.RegisterTarget(go, ps);
                                poiObserver.SetWorldCenter(arowDemoMain.GetWorldCenter());

                                for (int i = 0; i < test_json.Length; i++)
                                {
                                    PoiJSON j = JsonUtility.FromJson<PoiJSON>(test_json[i]);
                                    poiObserver.RegisterObservePoi(j);
                                    //					poiObserver.RegistrationNoticeDistance(test[i].category, test[i].subcategory, 50);
                                    Vector2 poi_pos = new Vector2(
                                        (ArowLibrary.ArowDefine.FlatBufferSchema.PositionUtil.ParseLatLon(j.longitude) - arowDemoMain.GetWorldCenter().x) * ArowMain.Runtime.CreateModelScripts.MapUtility.WorldScale.x,
                                        (ArowLibrary.ArowDefine.FlatBufferSchema.PositionUtil.ParseLatLon(j.latitude) - arowDemoMain.GetWorldCenter().y) * ArowMain.Runtime.CreateModelScripts.MapUtility.WorldScale.y
                                    );
                                    GameObject t = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                                    t.name = j.name;
                                    t.transform.localScale = new Vector3(3f, 300f, 3f);
                                    t.transform.localPosition = new Vector3(poi_pos.x, 300f, poi_pos.y);
                                }

                                {
                                    // 検出したい対象を好きに登録
                                    poiObserver.RegisterNoticeDistance("交通", "駅", 50);
                                    poiObserver.RegisterNoticeDistance("郵便局", "", 50);
                                    //					poiObserver.RegistrationNoticeDistance("店舗", "", 50);
                                    poiObserver.RegisterNoticeDistance("学校", "", 50);
                                    //			        poiObserver.RegistrationNoticeDistance("学校", "中学校", 50);
                                    poiObserver.RegisterNoticeDistance("コンビニ", "ファミリーマート", 50);
                                    poiObserver.RegisterNoticeDistance("コンビニ", "セブン-イレブン", 50);
                                    poiObserver.RegisterNoticeDistance("公共", "", 50);
                                }
                */
                state = STATE_AROW_MAP.PLAYING_GAME;
                break;

            case STATE_AROW_MAP.PLAYING_GAME:
                break;

            default:
                Debug.Assert(false);
                break;
        }

        StatusText.text = state.ToString();
    }



    private void OnGUI()
    {
        if (GUI.Button(new Rect(100, 50, 140, 50),  "距離登録\nカテゴリのみ"))
        {
            GameObject ob = GameObject.Find("PoiObserver");
            ArowPoiObserver poiObserver = ob.GetComponent<ArowPoiObserver>();
            poiObserver.RegisterNoticeDistance(
                ArowLibrary.ArowDefine.POIDefine.CategoryTbl[ArowLibrary.ArowDefine.POIDefine.CATEGORY.COMMERCIAL_FACILITY].name, 50);
        }

        if (GUI.Button(new Rect(100, 120, 140, 50),  "距離登録\nサブカテゴリまで"))
        {
            GameObject ob = GameObject.Find("PoiObserver");
            ArowPoiObserver poiObserver = ob.GetComponent<ArowPoiObserver>();
            poiObserver.RegisterNoticeDistance(
                ArowLibrary.ArowDefine.POIDefine.CategoryTbl[ArowLibrary.ArowDefine.POIDefine.CATEGORY.COMMERCIAL_FACILITY].name,
                ArowLibrary.ArowDefine.POIDefine.SubcategoryTbl[ArowLibrary.ArowDefine.POIDefine.SUBCATEGORY.FAST_FOOD].name, 50);
        }

        if (GUI.Button(new Rect(100, 190, 140, 50),  "距離登録\nサブサブカテゴリまで"))
        {
            GameObject ob = GameObject.Find("PoiObserver");
            ArowPoiObserver poiObserver = ob.GetComponent<ArowPoiObserver>();
            poiObserver.RegisterNoticeDistance(
                ArowLibrary.ArowDefine.POIDefine.CategoryTbl[ArowLibrary.ArowDefine.POIDefine.CATEGORY.COMMERCIAL_FACILITY].name,
                ArowLibrary.ArowDefine.POIDefine.SubcategoryTbl[ArowLibrary.ArowDefine.POIDefine.SUBCATEGORY.FAST_FOOD].name,
                "マクドナルド", 50);
        }

        if (GUI.Button(new Rect(100, 260, 140, 50),  "距離解除"))
        {
            GameObject ob = GameObject.Find("PoiObserver");
            ArowPoiObserver poiObserver = ob.GetComponent<ArowPoiObserver>();
            poiObserver.RegisterNoticeDistance(ArowLibrary.ArowDefine.POIDefine.CategoryTbl[ArowLibrary.ArowDefine.POIDefine.CATEGORY.COMMERCIAL_FACILITY].name, 0);
        }

        if (GUI.Button(new Rect(Screen.width - 100 - 50, 50, 100, 50),  "シーン再ロード"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_MoveControlCreatedMap", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        if (GUI.Button(new Rect(Screen.width - 100 - 50, Screen.height - 50 - 50, 100, 50), "スタートシーンへ"))
        {
            ArowSceneManager.ChangeScene(ArowSceneManager.StartSceneName);
        }
    }
}
}