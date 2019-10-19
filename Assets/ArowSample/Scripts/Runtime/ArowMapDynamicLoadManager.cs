using System.IO;
using UnityEngine;


namespace ArowSampleGame.Runtime
{

/// <summary>
/// 動的に地図を使用する為のクラス
/// このソースのみをGameObjectにアタッチして使用する
/// </summary>
public class ArowMapDynamicLoadManager : MonoBehaviour
{
    private ArowMapPosManager arowMapPosManager;
    private ArowMapObjectCreator arowMapObjectCreator;

    private int startLatitude;		// スタート地点の緯度
    private int startLongitude;		// スタート地点の経度

    // ファイル名に直結
    // 緯度　
    // 経度　0詰め3桁整数部と小数点第三位までの数字を組み合わせた数字がそのまま入っている
    private int currentLatitude;			// 緯度
    private int currentLongitude;			// 経度　

    private Vector2Int center;

    private const float LATLON_ABS_SCALE = 	10e6f;
    private const float LATITUDE_ABS_MAX = 	90f * LATLON_ABS_SCALE;		// 緯度の最大値（絶対値
    private const float LONGITUDE_ABS_MAX = 180f * LATLON_ABS_SCALE;		// 経度の最大値（絶対値

    private const int ADD_LATITUDE =  100000;			// 地図のサイズ　（緯度
    private const int ADD_LONGITUDE = 100000;		// 地図のサイズ　（経度

    public const float DYNAMIC_LOAD_DISTANCE = 150f;

    enum STATE
    {
        NONE,
        DOWN_LOAD,
    }

    // Use this for initialization
    void Start()
    {
        Initialize(GameObject.Find("Cube"), new Vector2Int(1396900000, 356500000));		// プレイヤーの位置情報で初期化
    }

    public void Initialize(GameObject monitoring_target_gameObject, Vector2Int target_latlon)
    {
        // 経度０緯度０から、現在の場所を探る
        //緯度から
        if (target_latlon.y >= 0)
        {
            for (int i = 0; i <= LATITUDE_ABS_MAX / ADD_LATITUDE; i++)
            {
                int lat = ADD_LATITUDE * (i + 1);

                if (target_latlon.y < lat)
                {
                    startLatitude = ADD_LATITUDE * i;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i <= LATITUDE_ABS_MAX / ADD_LATITUDE; i++)
            {
                int lat = -ADD_LATITUDE * (i + 1);

                if (target_latlon.y < lat)
                {
                    startLatitude = -ADD_LATITUDE * i;
                    break;
                }
            }
        }

        // 経度計算
        if (target_latlon.x >= 0)
        {
            for (int i = 0; i <= LONGITUDE_ABS_MAX / ADD_LONGITUDE; i++)
            {
                int lon = ADD_LONGITUDE * (i + 1);

                if (target_latlon.x < lon)
                {
                    startLongitude = ADD_LONGITUDE * i;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i <= LONGITUDE_ABS_MAX / ADD_LONGITUDE; i++)
            {
                int lon = -ADD_LONGITUDE * (i + 1);

                if (target_latlon.x < lon)
                {
                    startLongitude = -ADD_LONGITUDE * i;
                    break;
                }
            }
        }

        // マップが決定
        var upperPosition = new Vector2Int(startLongitude + ADD_LONGITUDE, startLatitude + ADD_LATITUDE);
        var lowerPosition = new Vector2Int(startLongitude, startLatitude);
        Debug.Log("[startLongitude:" + startLongitude + "][startLatitude:" +  startLatitude);
        center = ArowMain.Runtime.CreateModelScripts.MapUtility.CalculateWorldCenter(
                     startLongitude + ADD_LONGITUDE,
                     startLatitude + ADD_LATITUDE,
                     startLongitude,
                     startLatitude);
        arowMapPosManager = gameObject.AddComponent<ArowMapPosManager>();
        arowMapObjectCreator = gameObject.AddComponent<ArowMapObjectCreator>();
        arowMapPosManager.Initialized(monitoring_target_gameObject.transform, center, upperPosition, lowerPosition, DYNAMIC_LOAD_DISTANCE);
        arowMapObjectCreator.Initialize(center);
        {
            arowMapObjectCreator.CreateGround(startLongitude, startLatitude);
            arowMapObjectCreator.CreateRoad(startLongitude, startLatitude);
            arowMapObjectCreator.CreateBuilding(startLongitude, startLatitude);
        }
        currentLatitude = startLatitude;
        currentLongitude = startLongitude;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var key in arowMapPosManager.UpdateFlag.Keys)
        {
            // マップの「読み込み」をするのかチェック
            if (arowMapPosManager.UpdateFlag[key])
            {
                switch (key)
                {
                    case ArowMapPosManager.UPDATE_AREA.East:
                        arowMapObjectCreator.CreateGround(currentLongitude + ADD_LONGITUDE,	currentLatitude);
                        arowMapObjectCreator.CreateRoad(currentLongitude + ADD_LONGITUDE,	currentLatitude);
                        arowMapObjectCreator.CreateBuilding(currentLongitude + ADD_LONGITUDE,	currentLatitude);
                        break;

                    case ArowMapPosManager.UPDATE_AREA.West:
                        arowMapObjectCreator.CreateGround(currentLongitude - ADD_LONGITUDE,	currentLatitude);
                        arowMapObjectCreator.CreateRoad(currentLongitude - ADD_LONGITUDE,	currentLatitude);
                        arowMapObjectCreator.CreateBuilding(currentLongitude - ADD_LONGITUDE,	currentLatitude);
                        break;

                    case ArowMapPosManager.UPDATE_AREA.North:
                        arowMapObjectCreator.CreateGround(currentLongitude, 					currentLatitude + ADD_LATITUDE);
                        arowMapObjectCreator.CreateRoad(currentLongitude, 					currentLatitude + ADD_LATITUDE);
                        arowMapObjectCreator.CreateBuilding(currentLongitude, 					currentLatitude + ADD_LATITUDE);
                        break;

                    case ArowMapPosManager.UPDATE_AREA.South:
                        arowMapObjectCreator.CreateGround(currentLongitude,					currentLatitude - ADD_LATITUDE);
                        arowMapObjectCreator.CreateRoad(currentLongitude,					currentLatitude - ADD_LATITUDE);
                        arowMapObjectCreator.CreateBuilding(currentLongitude,					currentLatitude - ADD_LATITUDE);
                        break;
                }
            }

            // 「現在立っているマップ」を切り替える
            bool update_origin_pos = false;
            Vector2Int upperPosition = Vector2Int.zero;
            Vector2Int lowerPosition = Vector2Int.zero;

            if (arowMapPosManager.PosOverFlag[key])
            {
                update_origin_pos = true;

                switch (key)
                {
                    case ArowMapPosManager.UPDATE_AREA.East:
                        currentLongitude += ADD_LONGITUDE;
                        break;

                    case ArowMapPosManager.UPDATE_AREA.West:
                        currentLongitude -= ADD_LONGITUDE;
                        break;

                    case ArowMapPosManager.UPDATE_AREA.North:
                        currentLatitude += ADD_LATITUDE;
                        break;

                    case ArowMapPosManager.UPDATE_AREA.South:
                        currentLatitude -= ADD_LATITUDE;
                        break;
                }

                // 「マップ」を切り替え
                upperPosition = new Vector2Int(currentLongitude + ADD_LONGITUDE, currentLatitude + ADD_LATITUDE);
                lowerPosition = new Vector2Int(currentLongitude, currentLatitude);
            }

            // 更新
            if (update_origin_pos)
            {
                arowMapPosManager.SetOriginPoint(upperPosition, lowerPosition);
            }
        }
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(50, 50, 100, 50), "キャッシュクリア"))
        {
            // サーバーからダウンロードした地図データを削除
            var dirPath = Path.Combine(Application.temporaryCachePath, "arow_map");

            if (Directory.Exists(dirPath))
            {
                Directory.Delete(dirPath, true);
            }
        }
    }

}
}