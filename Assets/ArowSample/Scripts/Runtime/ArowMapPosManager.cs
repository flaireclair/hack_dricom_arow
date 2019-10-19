using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ArowSampleGame.Runtime
{


/// <summary>
/// 地図の動的ロードの位置管理クラス
/// </summary>
public class ArowMapPosManager : MonoBehaviour
{
    private Vector2 originPoint = Vector2.zero;
    private Vector2Int centerPos;
    private Transform targetObjTrans;
    private float targetDistance;

    private Vector2Int NorthEast;
    private Vector2Int SouthWest;

    public enum UPDATE_AREA
    {
        North,
        East,
        South,
        West
    }
    // 次に読み込む地図の方向指定用
    public Dictionary<UPDATE_AREA, bool> UpdateFlag = new Dictionary<UPDATE_AREA, bool>
    {
        { UPDATE_AREA.North, false },
        { UPDATE_AREA.East, false },
        { UPDATE_AREA.South, false },
        { UPDATE_AREA.West, false },
    };
    // マップの移動判定用
    public Dictionary<UPDATE_AREA, bool> PosOverFlag = new Dictionary<UPDATE_AREA, bool>
    {
        { UPDATE_AREA.North, false },
        { UPDATE_AREA.East, false },
        { UPDATE_AREA.South, false },
        { UPDATE_AREA.West, false },
    };


    /// <summary>
    /// 地図の動的ロードを行うための対象Objectなどの設定（初期化）
    /// </summary>
    /// <param name="target_transform">Target transform.監視対象のTransform</param>
    /// <param name="north_east">North east.地図の北東</param>
    /// <param name="south_west">South west.</param>
    /// <param name="target_distance">Target distance.</param>
    public void Initialized(Transform target_transform, Vector2Int center, Vector2Int north_east, Vector2Int south_west, float target_distance)
    {
        targetObjTrans = target_transform;
        SetOriginPoint(north_east, south_west);
        targetDistance = target_distance;
        centerPos = center;
    }

    /// <summary>
    /// 距離を測る起点をセットする関数
    /// </summary>
    /// <param name="north_east">地図の右上の緯度経度.</param>
    /// <param name="south_west">地図の左下の緯度経度.</param>
    public void SetOriginPoint(Vector2Int north_east, Vector2Int south_west)
    {
        NorthEast = north_east;
        SouthWest = south_west;
        originPoint = ArowMain.Runtime.CreateModelScripts.MapUtility.CalculateWorldCenter(north_east, south_west);
    }

    // フラグのリセット
    private void ResetUpdateFlag()
    {
        List<UPDATE_AREA> idList = new List<UPDATE_AREA>(UpdateFlag.Keys);

        foreach (var flag in  idList)
        {
            UpdateFlag[flag] = false;
            PosOverFlag[flag] = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (targetObjTrans == null)
        {
            return;
        }

        Vector2 s = new Vector2(targetObjTrans.localPosition.x, targetObjTrans.localPosition.z);
        Vector2 s2 = s - new Vector2((originPoint.x - centerPos.x) * ArowMain.Runtime.CreateModelScripts.MapUtility.WorldScale.x,
                                     (originPoint.y - centerPos.y) * ArowMain.Runtime.CreateModelScripts.MapUtility.WorldScale.y);
        Vector2 ne = new Vector2((NorthEast.x - centerPos.x) * ArowMain.Runtime.CreateModelScripts.MapUtility.WorldScale.x,
                                 (NorthEast.y - centerPos.y) * ArowMain.Runtime.CreateModelScripts.MapUtility.WorldScale.y);
        Vector2 sw = new Vector2((SouthWest.x - centerPos.x) * ArowMain.Runtime.CreateModelScripts.MapUtility.WorldScale.x,
                                 (SouthWest.y - centerPos.y) * ArowMain.Runtime.CreateModelScripts.MapUtility.WorldScale.y);
        ResetUpdateFlag();	// 毎フレームUpdateFlagをfalse

        // マップ中央から一定距離離れると、次のマップを読み込む為、どの方向の地図が必要なのか判定する
        if (s2.magnitude > targetDistance)
        {
            var axis = Vector3.Dot(Vector2.right, s2);
            var angle = Vector2.Angle(Vector2.up, s2)
                        * (axis < 0 ? -1 : 1) ;

            // どの方向にすすんでいるのか（どの方向のデータをロードするのか
            if (45 <= angle && angle <= 135)
            {
                UpdateFlag[UPDATE_AREA.East] = true;
            }

            if (-45 <= angle && angle <= 45)
            {
                UpdateFlag[UPDATE_AREA.North] = true;
            }

            if (-135 <= angle && angle <= -45)
            {
                UpdateFlag[UPDATE_AREA.West] = true;
            }

            if (-135 >= angle || angle >= 135)
            {
                UpdateFlag[UPDATE_AREA.South] = true;
            }

            // 地図の端っこを過ぎたので、「基準」とする場所を変更するために判定
            if (ne.x < s.x)
            {
                PosOverFlag[UPDATE_AREA.East] = true;
            }

            if (ne.y < s.y)
            {
                PosOverFlag[UPDATE_AREA.North] = true;
            }

            if (sw.x > s.x)
            {
                PosOverFlag[UPDATE_AREA.West] = true;
            }

            if (sw.y > s.y)
            {
                PosOverFlag[UPDATE_AREA.South] = true;
            }
        }
    }
}
}