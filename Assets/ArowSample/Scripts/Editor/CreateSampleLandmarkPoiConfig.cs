using UnityEngine;
using UnityEditor;
using ArowMain.Runtime;
using ArowLibrary.ArowDefine;

namespace ArowSample.Scripts.Editor
{
public class CreateSampleLandmarkPoiConfig
{
    /// <summary>
    /// サンプル用のLandmarkPoiConfigを作成する
    /// </summary>
    /// <returns></returns>
    public static LandmarkPoiConfig CreateAssetSampleData()
    {
        var config = ScriptableObject.CreateInstance<LandmarkPoiConfig>();
        config.List = new System.Collections.Generic.List<LandmarkPoiConfig.CategoryLandmark>();

        foreach (var i in LandmarkDefine.LandMarkTbl.Keys)
        {
            config.List.Add(new LandmarkPoiConfig.CategoryLandmark(i));

            // 「渋谷QFRONT」のみ、prefabを設定
            if (i == LandmarkDefine.LANDMARK.SHIBUYA_QFRONT)
            {
                int shibuya_qfront_index = config.List.Count - 1;
                config.List[shibuya_qfront_index].LandmarkPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ArowSample/OtherAssets/ConvertPrefab/oneScaled_qfront_01.prefab");
            }
        }

        return config;
    }
}

}
