using System.Collections.Generic;
using System.Linq;
using ArowLibrary.ArowDefine.SchemaWrapper;
using ArowMain.Runtime;
using UnityEngine;


    /// <summary>
    /// 指定したTransformにNodeMap間を移動させるためのクラス.
    /// </summary>
    public class MapTracer : MonoBehaviour
    {
        public NodeMap NodeMap
        {
            set
            {
                _nodeMap = value;
            }
        }
        private NodeMap _nodeMap = null;

        // 移動させるTransform
        private Transform _targetTransForm;
        private string nextNode;
        private LinkedList<string> currentRoute;

        public bool isFinished = false;
        public bool MoveFlag = false;

        private void UpdatePosition(Transform _transform)
        {
            if (currentRoute == null)
            {
                return;
            }

            if (nextNode == null)
            {
                nextNode = currentRoute.First.Value;
                _transform.position = _nodeMap[nextNode].Position;
                return;
            }

            var targetPosition = _nodeMap[nextNode].Position;
            var targetDir = targetPosition - _transform.position;

            if (0.5f < targetDir.magnitude)
            {
                _transform.position = _transform.position + targetDir.normalized * 0.8f;
                return;
            }

            var nextNodeKouho = currentRoute.Find(nextNode).Next;

            if (nextNodeKouho != null)
            {
                nextNode = nextNodeKouho.Value;
            }
            else
            {
                isFinished = true;
                currentRoute = null;
                nextNode = null;
            }
        }

        private void Update()
        {
            if (_targetTransForm != null && MoveFlag)
            {
                UpdatePosition(_targetTransForm);
            }
        }

        /// <summary>
        /// _transform に道を歩かせる.
        /// </summary>
        /// <param name="_transform">Transform.</param>
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

            isFinished = false;
            MoveFlag = true;
            currentRoute = route;
        }
    }

