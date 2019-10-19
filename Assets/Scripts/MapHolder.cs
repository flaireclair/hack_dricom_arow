using System.Collections.Generic;
using ArowLibrary.ArowDefine.SchemaWrapper;
using ArowMain.Runtime;
using UnityEngine;

/// <summary>
/// 道のノード情報を保持するクラス
/// </summary>
public class MapHolder
{
    /// <summary>
    /// The node map.
    /// </summary>
    public NodeMap nodeMap
    {
        get
        {
            return _nodeMap;
        }
    }

    private NodeMap _nodeMap = null;

    /// <summary>
    /// 指定された点にもっとも近いノードを取得する
    /// </summary>
    /// <returns>The locate.</returns>
    /// <param name="selectPos">Select position.</param>
    /// <param name="groupIndex">Group index.</param>
    public string Locate(Vector3 selectPos, string groupIndex = null)
    {
        NodeMapUtility.GroupDict group;
        float distance = -1f;
        string key = null;

        if (string.IsNullOrEmpty(groupIndex))
        {
            foreach (var p in nodeMap.nodeLonLatData.Keys)
            {
                Vector3 sub = nodeMap[p].Position - selectPos;
                float d = sub.x * sub.x + sub.y * sub.y + sub.z * sub.z;

                if (distance < 0f || distance > d)
                {
                    distance = d;
                    key = p;
                }
            }
        }
        else
        {
            NodeMapUtility.Group(nodeMap, out group);
            int y = group[groupIndex];

            foreach (var p in group)
            {
                if (p.Value == y)
                {
                    Vector3 sub = nodeMap[p.Key].Position - selectPos;
                    float d = sub.x * sub.x + sub.y * sub.y + sub.z * sub.z;

                    if (distance < 0f || distance > d)
                    {
                        distance = d;
                        key = p.Key;
                    }
                }
            }
        }

        Debug.Assert(distance >= 0f);
        return key;
    }


    /// <summary>
    /// 道の情報を NodeMap に変換する.
    /// </summary>
    /// <param name="roadDataModels">Road data models.</param>
    /// <param name="worldCenter">World center.</param>
    /// <param name="worldScale">World scale.</param>
    /// <param name="key">AROWMAPから抽出するTAG.</param>
    public void Initialize(List<RoadDataModel> roadDataModels, Vector2Int worldCenter, Vector2 worldScale, OSMDefine.CREATE_WAY key)
    {
        _nodeMap = NodeMapUtility.CreateNodeMap(roadDataModels, worldCenter, worldScale, key);
    }


}
