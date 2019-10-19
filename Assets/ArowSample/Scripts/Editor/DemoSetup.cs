using System.IO;
using System.Linq;
using ArowMain.Editor;
using UnityEditor;
using UnityEngine;

namespace ArowSample.Scripts.Editor
{

public class DemoSetup
{
    [MenuItem("ArowSample/Setup for Demo", false, MenuItemProperty.ArowSampleDemoSetupGroup)]
    private static void CreateAsset_Normal()
    {
        // フォルダがあるかチェック、なければ打ち切り
        var path = "Assets/ArowSample/Resources/Demo/";

        if (!Directory.Exists(path))
        {
            Debug.LogWarning(path + " が存在しません。");
            return;
        }

        // oneScaled_ 系リソースの生成
        CreateOneScaledPrefabs();
        // ユニティちゃんが走り回るデモ用
        var config = BuildingConfigCreator.CreateNormalCreateConfig();
        AssetDatabase.CreateAsset(config, Path.Combine(path, "BuildingConfig.asset"));
        var poi_config = CreateSampleCategoryPoiConfig.CreateAssetSampleData();
        AssetDatabase.CreateAsset(poi_config, Path.Combine(path, "CategoryPoiConfig_DemoSample.asset"));
        var landmark_config = CreateSampleLandmarkPoiConfig.CreateAssetSampleData();
        AssetDatabase.CreateAsset(landmark_config, Path.Combine(path, "LandmarkPoiConfig_DemoSample.asset"));
        // ArowMap 用
        var prefab_config = CreateSamplePrefabConfigList.CreateAssetSampleData();
        AssetDatabase.CreateAsset(prefab_config, Path.Combine(path, "PrefabConfigList_demo.asset"));
        AssetDatabase.Refresh();
    }

    public static void CreateOneScaledPrefabs()
    {
        var paths = Directory.GetFiles("Assets/ArowSample/OtherAssets/ConvertPrefab/original/prefab")
                    .Where(p => p.EndsWith(".prefab")).ToArray();
        PrefabEditor.ConvertToOneMeterScalePrefab(paths, "Assets/ArowSample/OtherAssets/ConvertPrefab/");
    }
}

}
