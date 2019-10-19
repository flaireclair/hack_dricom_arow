using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ArowSample.Scripts.Runtime;


namespace ArowSampleGame.SampleScripts
{
public class ArowSampleGameMain_RouteSearch : MonoBehaviour
{
    enum STATE_AROW_MAP
    {
        NONE,
        DOWNLOAD_AROW_MAP,		// サーバーからarowmapデータをダウンロード.
        LOADING_AROW_MAP,		// ダウンロードしたデータの読み込み.
        CREATE_ROAD,			// 道の生成.
        CREATE_BUILDING,		// 建物の生成.
        SELECT_GOAL_POINT,
        READY_GAME,				// 「スタート」待ち.
        PLAYING_GAME,			//
    };

    // 移動し終わった後の待ち時間（秒）
    private const float WAIT_TIME = 3f;

    private bool[] isStateEnd;
    private Camera mainCamera;
    private Camera menuCamera;

    private float waitTime = WAIT_TIME;

    // 道情報保持クラス
    private NodeMapTracer nodeMapTracer;
    private NodeMapHolder nodeMapHolder;
    private STATE_AROW_MAP state = STATE_AROW_MAP.NONE;
    string startNodeKeyName = null;
    string goalNodeKeyName = null;
    private GameObject startObj = null;
    private GameObject goalObj = null;
    private MeshRenderer startMtl = null;
    private MeshRenderer goalMtl = null;

    private ArowDemoMain arowDemoMain;

    // Setting Inspector
    public Text StatusText;
    public GameObject PlayOrderTextObj;
    public GameObject PleaseWaitTextObj;
    public Button StartBtn;
    public GameObject WarkerObj;

    // Use this for initialization
    void Start()
    {
        Initialize();
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
                SetupMenuCamera();
                state = STATE_AROW_MAP.CREATE_ROAD;
                break;

            case STATE_AROW_MAP.CREATE_ROAD:
                nodeMapHolder = arowDemoMain.CreateRoads();
                SetupNodeMapTracer();
                state = STATE_AROW_MAP.CREATE_BUILDING;
                break;

            case STATE_AROW_MAP.CREATE_BUILDING:
                if (arowDemoMain.IsEnd[ArowDemoMain.MAP_CREATE.ROAD])
                {
                    arowDemoMain.CreateBuildingsNonCollider();
                    state = STATE_AROW_MAP.SELECT_GOAL_POINT;
                    StartBtn.gameObject.SetActive(true);
                    StartBtn.interactable = false;
                    mainCamera.enabled = false;
                    menuCamera.enabled = true;
                    PleaseWaitTextObj.SetActive(false);
                    PlayOrderTextObj.SetActive(true);
                }

                break;

            case STATE_AROW_MAP.SELECT_GOAL_POINT:
                ClickMap();
                break;

            case STATE_AROW_MAP.READY_GAME:
                break;

            case STATE_AROW_MAP.PLAYING_GAME:
                if (nodeMapTracer.isFinished)
                {
                    waitTime -= Time.deltaTime;

                    if (waitTime <= 0f)
                    {
                        ResetPlayingGame();		// 再度、ゴールポイント選択のStateへ
                    }
                }

                break;

            default:
                Debug.Assert(false);
                break;
        }

        StatusText.text = state.ToString();
    }

    private void OnGUI()
    {
        switch (state)
        {
            case STATE_AROW_MAP.PLAYING_GAME:
                if (nodeMapTracer != null)
                {
                    if (GUI.Button(new Rect(50, 50, 100, 50), nodeMapTracer.MoveFlag ? "Unityちゃん\n停止" : "Unityちゃん\n移動再開"))
                    {
                        nodeMapTracer.MoveFlag = !nodeMapTracer.MoveFlag;
                    }
                }

                break;
        }

        if (GUI.Button(new Rect(Screen.width - 100 - 50, Screen.height - 50 - 50, 100, 50), "スタートシーンへ"))
        {
            ArowSceneManager.ChangeScene(ArowSceneManager.StartSceneName);
        }
    }

    // 道関連のデータを保持者とトレーサーにセット
    public void SetupNodeMapTracer()
    {
        Debug.Assert(nodeMapHolder != null);
        nodeMapTracer = gameObject.GetComponent<NodeMapTracer>();

        if (nodeMapTracer == null)
        {
            nodeMapTracer = gameObject.AddComponent<NodeMapTracer>();
        }

        Debug.Assert(nodeMapHolder.nodeMap != null);
        nodeMapTracer.NodeMap = nodeMapHolder.nodeMap;
    }

    // uGUIボタンで「スタート」を押した
    public void OnClickStart()
    {
        if (!string.IsNullOrEmpty(startNodeKeyName) && !string.IsNullOrEmpty(goalNodeKeyName))
        {
            nodeMapTracer.MoveStartShortestRoute(WarkerObj.transform, startNodeKeyName, goalNodeKeyName);
            StartBtn.gameObject.SetActive(false);
            state = STATE_AROW_MAP.PLAYING_GAME;
            mainCamera.enabled = true;
            menuCamera.enabled = false;
            startObj.SetActive(false);
            goalObj.SetActive(false);
            PlayOrderTextObj.SetActive(false);
        }
    }

    private void Initialize()
    {
        StartBtn.gameObject.SetActive(false);
        state = STATE_AROW_MAP.DOWNLOAD_AROW_MAP;
        isStateEnd = new bool[Enum.GetNames(typeof(STATE_AROW_MAP)).Length];

        for (int i = 0; i < isStateEnd.Length; i++)
        {
            isStateEnd[i] = false;
        }

        mainCamera = Camera.main;
        menuCamera = GameObject.Find("Camera").GetComponent<Camera>();
        PlayOrderTextObj.SetActive(false);
        arowDemoMain = gameObject.AddComponent<ArowDemoMain>();
    }
    /// <summary>
    /// AROWMAPのデータに合わせて、経路探索位置指定用カメラのセットアップを行う
    /// </summary>
    private void SetupMenuCamera()
    {
        Debug.Assert(arowDemoMain != null);
        Debug.Assert(menuCamera != null);
        menuCamera.orthographicSize = arowDemoMain.OrthographicSize;
        menuCamera.gameObject.transform.localPosition = new Vector3(0f, 200f, 0f);
    }

    /// <summary>
    /// マップをクリックした際の挙動実装
    /// </summary>
    private void ClickMap()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // クリックした場所に一番近い道のノードを検索する
            RaycastHit hit;

            if (Physics.Raycast(menuCamera.ScreenPointToRay(Input.mousePosition), out hit, 300))
            {
                List<RaycastResult> raycastResults = new List<RaycastResult>();
                PointerEventData eventDataCurrent = new PointerEventData(EventSystem.current);
                eventDataCurrent.position = Input.mousePosition;
                EventSystem.current.RaycastAll(eventDataCurrent, raycastResults);

                // 「uGUI」に触れていたら、無視をする
                // （スタート、ゴールの設定をしない
                if (raycastResults.Count <= 0)
                {
                    var d = hit.point;
                    startNodeKeyName = goalNodeKeyName;
                    goalNodeKeyName = nodeMapHolder.Locate(hit.point, startNodeKeyName);
                    GameObject tmp = startObj;
                    startObj = goalObj;
                    goalObj = tmp;

                    // クリックして選ばれたノードが常に「ゴール」となるように入れ替える
                    // 「新しく選ばれたノード」→「新しいゴール」
                    // 「古いゴール」→「新しいスタート」
                    // 「古いスタート」→ 破棄
                    if (startMtl != null && goalMtl != null)
                    {
                        Material tmpMtr = startMtl.material;
                        startMtl.material = goalMtl.material;
                        goalMtl.material = tmpMtr;
                    }

                    // クリックした箇所に赤、青の球体を用意する
                    // 初回クリック時は、青い球体。移行は赤い球体が表示されるようにする
                    if (goalObj == null)	// 雑判定
                    {
                        GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        s.GetComponent<SphereCollider>().enabled = false;
                        Material m;

                        // 初回は「青」になるようにする
                        if (startObj == null)
                        {
                            m = Instantiate(Resources.Load<Material>("Demo/StartObj"));
                            startMtl = s.GetComponent<MeshRenderer>();
                            startMtl.material = m;
                        }
                        else
                        {
                            StartBtn.interactable = true;
                            m = Instantiate(Resources.Load<Material>("Demo/GoalObj"));
                            goalMtl = s.GetComponent<MeshRenderer>();
                            goalMtl.material = m;
                        }

                        goalObj = s;
                    }

                    goalObj.transform.localScale = new Vector3(60f, 60f, 60f);
                    goalObj.transform.localPosition = nodeMapHolder.nodeMap[goalNodeKeyName].Position;
                }
            }
        }
    }

    void ResetPlayingGame()
    {
        StartBtn.gameObject.SetActive(true);
        state = STATE_AROW_MAP.SELECT_GOAL_POINT;
        mainCamera.enabled = false;
        menuCamera.enabled = true;
        startObj.SetActive(true);
        goalObj.SetActive(true);
        PlayOrderTextObj.SetActive(true);
        waitTime = WAIT_TIME;
    }
}

}