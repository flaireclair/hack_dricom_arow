using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// POIObserverから呼ばれるIArowPoiVisitorのインターフェース実装のサンプルソース
/// </summary>
public class PoiVisitorSample : MonoBehaviour, ArowMain.Runtime.IArowPoiVisitor
{
    private Camera targetCamera;
    private GameObject exclamation = null;
    private Text speechBubble = null;

    // 「！！」が表示される時間（秒）
    private const float EXCLAMATION_RESET_TIME = 3f;

    // デフォルトコメント
    private const string UNITY_CHAN_COMMENT_NORMAL = "お腹減った・・・・・。\nどこかお店でもないかな。";
    private const string UNITY_CHAN_COMMENT_AREA_IN = "クンクン！この匂いは!!\n(ファストフード店まで、\n\tあと{0}m)";
    private const string UNITY_CHAN_COMMENT_CLOSE_SHOP = "ファストフード店に到着!!";

    // POIのオブジェクトに「接触した」と判定する距離（メートル）
    private const float CLOSE_BORDER_DISTANCE = 12;

    // 「！！」の出る場所のoffset
    private Vector3 offset = new Vector3(0f, 1.8f, 0f);
    private float exclamationTimer = EXCLAMATION_RESET_TIME;
    // Use this for initialization
    void Start()
    {
        if (this.targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        speechBubble = GameObject.Find("Canvas/Image/Text").GetComponent<Text>();
        speechBubble.text = UNITY_CHAN_COMMENT_NORMAL;
        exclamation = Instantiate(Resources.Load<GameObject>("Demo/Exclamation"));
        exclamation.transform.SetParent(GameObject.Find("Walkman_unitychan_sample").transform);
        exclamation.transform.localPosition = offset;
        exclamation.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // 「！！」をカメラの方に向ける（上下の向きは無視）
        Vector3 target = this.targetCamera.transform.position;
        target.y = this.transform.position.y;
        exclamation.transform.LookAt(target);

        // 「！！」の表示時間をカウント
        if (exclamationTimer > 0)
        {
            exclamationTimer -= Time.deltaTime;

            if (exclamationTimer <= 0)
            {
                exclamation.SetActive(false);
            }
        }
    }

    /// <summary>
    /// POIObserverに登録したPOIに近づいた場合に呼ばれる関数
    /// （IArowPoiVisitorのインターフェース関数の実装）
    /// </summary>
    /// <param name="poi_list">「近づいた」という判定になっているPOIの配列</param>
    public void OnAreaEnter(ArowMain.Runtime.ArowPoi[] poi_list)
    {
        // LOGに表示
        for (int i = 0; i < poi_list.Length; i++)
        {
            Debug.LogFormat("OnAreaEnter:{0}:{1}:{2}:", poi_list[i].category, poi_list[i].subcategory, poi_list[i].name);
        }

        // POIが「商業施設」「ファストフード」の場合に「！！」を表示させる
        for (int i = 0; i < poi_list.Length; i++)
        {
            ArowMain.Runtime.ArowPoiCategory poiCategory = new ArowMain.Runtime.ArowPoiCategory(poi_list[i].category, poi_list[i].subcategory, null);

            if (poiCategory.Match(ArowLibrary.ArowDefine.POIDefine.CategoryTbl[ArowLibrary.ArowDefine.POIDefine.CATEGORY.COMMERCIAL_FACILITY].name,
                                  ArowLibrary.ArowDefine.POIDefine.SubcategoryTbl[ArowLibrary.ArowDefine.POIDefine.SUBCATEGORY.FAST_FOOD].name))
            {
                exclamationTimer = EXCLAMATION_RESET_TIME;
                exclamation.SetActive(true);
            }
        }
    }
    /// <summary>
    /// POIObserverに登録したPOIの範囲内にいる場合に呼ばれる関数
    /// （IArowPoiVisitorのインターフェース関数の実装）
    /// </summary>
    /// <param name="poi_list">「近づいた」という判定になっているPOIの配列</param>
    public void OnAreaIn(ArowMain.Runtime.ArowPoi[] poi_list)
    {
        // LOGに表示
        for (int i = 0; i < poi_list.Length; i++)
        {
            Debug.LogFormat("OnAreaIn:{0}:{1}:{2}:", poi_list[i].category, poi_list[i].subcategory, poi_list[i].name);
        }

        float dist = float.MaxValue;
        bool isDisp = false;

        // POIが「商業施設」「ファストフード」の場合、セリフの変更とその距離を表示する
        for (int i = 0; i < poi_list.Length; i++)
        {
            ArowMain.Runtime.ArowPoiCategory poiCategory = new ArowMain.Runtime.ArowPoiCategory(poi_list[i].category, poi_list[i].subcategory, null);

            if (poiCategory.Match(ArowLibrary.ArowDefine.POIDefine.CategoryTbl[ArowLibrary.ArowDefine.POIDefine.CATEGORY.COMMERCIAL_FACILITY].name,
                                  ArowLibrary.ArowDefine.POIDefine.SubcategoryTbl[ArowLibrary.ArowDefine.POIDefine.SUBCATEGORY.FAST_FOOD].name))
            {
                // 距離の計算
                Vector2 poi_pos = new Vector2(poi_list[i].longitude_x, poi_list[i].latitude_z);
                Vector2 unity_chan_pos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.z);
                Vector2 sub = poi_pos - unity_chan_pos;

                if (dist > sub.magnitude)
                {
                    dist = sub.magnitude;
                    isDisp = true;
                }
            }

            // 距離に応じて、表示内容を変える
            if (isDisp)
            {
                if (dist > CLOSE_BORDER_DISTANCE)
                {
                    speechBubble.text = string.Format(UNITY_CHAN_COMMENT_AREA_IN, dist.ToString("##.0"));
                }
                else if (dist <= CLOSE_BORDER_DISTANCE)
                {
                    speechBubble.text = string.Format(UNITY_CHAN_COMMENT_CLOSE_SHOP);
                }
            }
        }
    }
    /// <summary>
    /// POIObserverに登録したPOIの範囲から、外に出てしまった時に呼ばれる関数
    /// （IArowPoiVisitorのインターフェース関数の実装）
    /// </summary>
    /// <param name="poi_list">離れてしまったPOIの配列</param>
    public void OnAreaExit(ArowMain.Runtime.ArowPoi[] poi_list)
    {
        // LOGの表示
        for (int i = 0; i < poi_list.Length; i++)
        {
            Debug.LogFormat("OnAreaOut:{0}:{1}:{2}:", poi_list[i].category, poi_list[i].subcategory, poi_list[i].name);
        }

        // POIが「商業施設」「ファストフード」の場合、セリフを変更する
        for (int i = 0; i < poi_list.Length; i++)
        {
            ArowMain.Runtime.ArowPoiCategory poiCategory = new ArowMain.Runtime.ArowPoiCategory(poi_list[i].category, poi_list[i].subcategory, null);

            if (poiCategory.Match(ArowLibrary.ArowDefine.POIDefine.CategoryTbl[ArowLibrary.ArowDefine.POIDefine.CATEGORY.COMMERCIAL_FACILITY].name,
                                  ArowLibrary.ArowDefine.POIDefine.SubcategoryTbl[ArowLibrary.ArowDefine.POIDefine.SUBCATEGORY.FAST_FOOD].name))
            {
                speechBubble.text = UNITY_CHAN_COMMENT_NORMAL;
            }
        }
    }


}
