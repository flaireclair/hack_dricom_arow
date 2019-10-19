using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArowMain.Runtime;
using UnityEngine.EventSystems;
using ArowLibrary.ArowDefine.SchemaWrapper;

public class FuneMove : MonoBehaviour
{
    private Transform _targetTransForm;
    private string nextNode;
    string startNodeKeyName = null;
    string goalNodeKeyName = null;
    private NodeMap _nodeMap = null;
    private Camera mainCamera;
    private NodeMapHolder nodeMapHolder;
    private GameObject startObj = null;
    private LinkedList<string> currentRoute;
    private GameObject goalObj = null;
    private MeshRenderer startMtl = null;
    private MeshRenderer goalMtl = null;
    


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public class NodeMapHolder
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

    public void MoveStartShortestRoute(Transform _transform, string startKey, string goalKey)
    {
        _targetTransForm = _transform;
        NodeMapUtility.CostDict keyValuePairs;
        LinkedList<string> route;
        NodeMapUtility.GetShortestRoute(_nodeMap, startKey, goalKey, 10000, out keyValuePairs, out route);
        Debug.Log(string.Format("道を探します : start : {0} {1}", startKey, goalKey));

        if (route == null)
        {
            Debug.Log("道が遠すぎたため停止します");
            currentRoute = null;
            return;
        }

    }

    private void ClickMap()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // クリックした場所に一番近い道のノードを検索する
            RaycastHit hit;

            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 300))
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
                            // StartBtn.interactable = true;
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
}
