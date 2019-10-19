using System;
using System.IO;
using ArowMain.Editor;
using ArowMain.Runtime;
using UnityEditor;
using UnityEngine;

namespace ArowSample.Scripts.Editor
{

public class BuildingConfigCreator
{

    [MenuItem("ArowSample/Create BuildingConfig", false, MenuItemProperty.ArowSampleBuildingConfigGroup)]
    private static void CreateAsset_Normal()
    {
        var config = CreateNormalCreateConfig();
        AssetCreationUtils.CreateAsset(config, "BuildingConfig.asset");
    }

    [MenuItem("ArowSample/Create BuildingConfig (Interior Mapping)", false, MenuItemProperty.ArowSampleBuildingConfigGroup)]
    private static void CreateAsset_InteriorMapping()
    {
        var config = ScriptableObject.CreateInstance<CreateConfig>();
        config.FilePath = "Assets/StreamingAssets/meguro.arowmap";
        config.BuildingWallMaterials.AddRange(Resources.LoadAll<Material>("InteriorMapping/Assets/Shaders"));
        config.BuildingRoofMaterials.AddRange(Resources.LoadAll<Material>("Buildings/Roofs/FlatMaterials/"));

        switch (config.FillGapGroundElement)
        {
            case CreateConfig.FillGapGround.LiftupBuilding:
                config.BuildingFillGapMaterials.AddRange(Resources.LoadAll<Material>("Buildings/FillGap/"));
                break;
        }

        AssetCreationUtils.CreateAsset(config, "BuildingConfigInteriorMapping.asset");
    }

    [MenuItem("ArowSample/Create BuildingConfig (Prefab)", false, MenuItemProperty.ArowSampleBuildingConfigGroup)]
    private static void CreateAsset_Prefab()
    {
        var config = ScriptableObject.CreateInstance<PrefabConfigList>();
        config.FilePath = "Assets/StreamingAssets/meguro.arowmap";
        // 記述が長いので置き換え
        Func<string, GameObject> loadPrefab = (name) =>
        {
            var path = Path.Combine("Assets/ArowSample/OtherAssets/ConvertPrefab/", name);
            return AssetDatabase.LoadAssetAtPath<GameObject>(path);
        };
        config.LargeObjects.Add(loadPrefab("oneScaled_P01_A.prefab"));
        config.MiddleObjects.Add(loadPrefab("oneScaled_P04_A.prefab"));
        config.SmallObjects.Add(loadPrefab("oneScaled_P04_D.prefab"));

        if (config.LargeObjects[0] == null || config.MiddleObjects[0] == null || config.SmallObjects[0] == null)
        {
            Debug.LogError("prefab の読み込みに失敗しました。エディター拡張の Setup for Demo で再生成ができます。");
        }

        config.LargeScale = 60;
        config.MiddleScale = 30;
        config.Scale = 0.6f;
        config.FillGapGroundElement = CreateConfig.FillGapGround.LiftupBuilding;
        config.FillGapGroundMaterilal.AddRange(Resources.LoadAll<Material>("Buildings/FillGap/"));
        AssetCreationUtils.CreateAsset(config, "BuildingConfigPrefab.asset");
    }

    public static CreateConfig CreateNormalCreateConfig()
    {
        var config = ScriptableObject.CreateInstance<CreateConfig>();
        config.FilePath = "Assets/StreamingAssets/meguro.arowmap";
        config.MeshPatternElement = CreateConfig.MeshPattern.NineMesh;
        config.BuildingWallSliceBottomMaterials.AddRange(Resources.LoadAll<Material>("Buildings/Walls/sliced_bottom"));
        config.BuildingWallMaterials.AddRange(Resources.LoadAll<Material>("Buildings/Walls/sliced_mid"));
        config.BuildingWallSliceTopMaterials.AddRange(Resources.LoadAll<Material>("Buildings/Walls/sliced_top"));
        config.BuildingRoofMaterials.AddRange(Resources.LoadAll<Material>("Buildings/Roofs/FlatMaterials/"));

        switch (config.FillGapGroundElement)
        {
            case CreateConfig.FillGapGround.LiftupBuilding:
                config.BuildingFillGapMaterials.AddRange(Resources.LoadAll<Material>("Buildings/FillGap/"));
                break;
        }

        return config;
    }

}

}
