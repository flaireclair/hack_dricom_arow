using System;
using System.Collections.Generic;
using ArowLibrary.ArowDefine.SchemaWrapper;
using ArowMain.Runtime;
using ArowMain.Runtime.CreateModelScripts;
using UnityEngine;

namespace ArowSample.Scripts.Runtime
{
public class CreateRuntimeBuildingBuilder : MonoBehaviour
{
    public void CreateBuildings(ArowMapObjectModel arowMapObjectModel)
    {
        CreateConfig config = CreateConfig.CreateInstance<CreateConfig>();
        config.BuildingWallMaterials.Add(Resources.Load<Material>("Buildings/Walls/wide_mat"));
        config.BuildingRoofMaterials.AddRange(Resources.LoadAll<Material>("Buildings/Roofs/FlatMaterials/"));
        SetupFillGapGroundElement(config);
        CreateBuildings(arowMapObjectModel, config);
    }

    public void CreateBuildingsByNineSlice(ArowMapObjectModel arowMapObjectModel)
    {
        CreateConfig config = CreateConfig.CreateInstance<CreateConfig>();
        SetupConfigForSampleSlice(config);
        SetupFillGapGroundElement(config);
        // 上中下のスライス画像間で組み合わせを固定する（各画像の数は固定という前提）
        config.SetupSharedMaterials = GetFixTextureSuiteCallbackForSampleSlice(config);
        CreateBuildings(arowMapObjectModel, config);
    }

    public void CreateBuildingsByInteriorMapping(ArowMapObjectModel arowMapObjectModel)
    {
        CreateConfig config = CreateConfig.CreateInstance<CreateConfig>();
        config.BuildingDrawTypeElement = CreateConfig.BuildingDrawType.InteriorMapping;
        config.BuildingWallMaterials.AddRange(Resources.LoadAll<Material>("InteriorMapping/Assets/Shaders"));
        config.BuildingRoofMaterials.AddRange(Resources.LoadAll<Material>("Buildings/Roofs/FlatMaterials/"));
        SetupFillGapGroundElement(config);
        CreateBuildings(arowMapObjectModel, config);
    }

    public void CreatePrefabBuildings(ArowMapObjectModel arowMapObjectModel, PrefabConfigList configList)
    {
        var nodeMapHolderParentInfo = CreateRuntimeUtility.GetOrCreateParentInfoFromArowMapObjectModel(arowMapObjectModel);
        CreatePrefabBuilding(arowMapObjectModel.BuildingDataModels,
                             arowMapObjectModel.RoadDataModels,
                             nodeMapHolderParentInfo,
                             nodeMapHolderParentInfo.WorldCenter,
                             nodeMapHolderParentInfo.WorldScale,
                             configList);
    }

    private void CreateBuildings(ArowMapObjectModel arowMapObjectModel, CreateConfig config)
    {
        var nodeMapHolderParentInfo = CreateRuntimeUtility.GetOrCreateParentInfoFromArowMapObjectModel(arowMapObjectModel);
        // BuildingのMesh生成
        var buildingMeshCreator =
            new BuildingCreator
        .Builder(arowMapObjectModel.BuildingDataModels)
        .SetWorldCenter(nodeMapHolderParentInfo.WorldCenter)
        .SetWorldScale(nodeMapHolderParentInfo.WorldScale)
        .SetConfig(config)
        .IsExtractWithTaskAsync(true)
        .SetOnMeshBuildType(BuildingCreator.BUILD_TYPE.BUILDING)
        .SetOnMeshCreatedCallBack((BuildingDataModelWithMesh buildingDataWithMesh) =>
        {
            // Meshが生成されるごとに、GameObjectを生成する
            CreateBuildingMeshScripts.CreateBuildingGameObject(buildingDataWithMesh, nodeMapHolderParentInfo, config);
        })
        .SetOnCompletedCreateMeshCallBack((List<BuildingDataModelWithMesh> list) =>
        {
            // 処理時間計測停止はタスク内で行う
            MeasureProcessTime.Stop();
        });
        // 処理時間計測開始（停止は「SetOnCompletedCreateMeshCallBack」で行われるはず
        MeasureProcessTime.Start(MeasureProcessTime.Key.Building, "CreateBuildings");
        buildingMeshCreator.Build();
    }

    public void CreateWater(ArowMapObjectModel arowMapObjectModel)
    {
        var nodeMapHolderParentInfo = CreateRuntimeUtility.GetOrCreateParentInfoFromArowMapObjectModel(arowMapObjectModel);
        // BuildingのMesh生成
        var buildingMeshCreator = new BuildingCreator
        .Builder(arowMapObjectModel.BuildingDataModels)
        .SetWorldCenter(nodeMapHolderParentInfo.WorldCenter)
        .SetWorldScale(nodeMapHolderParentInfo.WorldScale)
        .IsExtractWithTaskAsync(true)
        .SetOnMeshBuildType(BuildingCreator.BUILD_TYPE.WATER_AREA)
        .SetOnMeshCreatedCallBack((BuildingDataModelWithMesh buildingDataWithMesh) =>
        {
            // Meshが生成されるごとに、GameObjectを生成する
            CreateBuildingMeshScripts.CreateWaterAreaGameObject(buildingDataWithMesh, nodeMapHolderParentInfo);
        })
        .SetOnCompletedCreateMeshCallBack((List<BuildingDataModelWithMesh> list) =>
        {
            // 処理時間計測停止はタスク内で行う
            MeasureProcessTime.Stop();
        });
        MeasureProcessTime.Start(MeasureProcessTime.Key.WaterArea, "CreateWater");
        buildingMeshCreator.Build();
    }

    private void CreatePrefabBuilding(List<BuildingDataModel> buildingDataSet, List<RoadDataModel> roadDataModelsSet, ParentInfo parent, Vector2Int worldCenter, Vector2 worldScale, PrefabConfigList prefabList)
    {
        var buildingPrefabCreator = new BuildingPrefabCreator
        .Builder(buildingDataSet)
        .SetRoadDataModels(roadDataModelsSet)
        .SetParentGameObject(parent.gameObject)
        .SetWorldCenter(worldCenter)
        .SetWorldScale(worldScale)
        .SetPrefabConfigList(prefabList)
        .SetOnCompletedCreateMeshCallBack((List<GameObject> list) =>
        {
            // 処理時間計測停止はタスク内で行う
            MeasureProcessTime.Stop();
        });
        MeasureProcessTime.Start(MeasureProcessTime.Key.Prefab, "CreatePrefab");
        buildingPrefabCreator.Build();
    }

    /// <summary>
    /// ビルをサンプルリソースでタイリング描画するための設定を行う
    /// </summary>
    /// <param name="config"></param>
    public static void SetupConfigForSampleSlice(CreateConfig config)
    {
        if (config)
        {
            config.MeshPatternElement = CreateConfig.MeshPattern.NineMesh;
            config.BuildingWallSliceBottomMaterials.AddRange(Resources.LoadAll<Material>("Buildings/Walls/sliced_bottom"));
            config.BuildingWallMaterials.AddRange(Resources.LoadAll<Material>("Buildings/Walls/sliced_mid"));
            config.BuildingWallSliceTopMaterials.AddRange(Resources.LoadAll<Material>("Buildings/Walls/sliced_top"));
            config.BuildingRoofMaterials.AddRange(Resources.LoadAll<Material>("Buildings/Roofs/FlatMaterials/"));
        }
    }

    /// <summary>
    /// 建物と地面の隙間を埋める Material を設定.
    /// 設定の内容によっては何もしないこともある
    /// </summary>
    /// <param name="config">Config.</param>
    public static void SetupFillGapGroundElement(CreateConfig config)
    {
        if (config)
        {
            switch (config.FillGapGroundElement)
            {
                case CreateConfig.FillGapGround.LiftupBuilding:
                    config.BuildingFillGapMaterials.AddRange(Resources.LoadAll<Material>("Buildings/FillGap/"));
                    break;
            }
        }
    }


    /// <summary>
    /// ビル描画に用いるスライステクスチャを固定した組み合わせで設定する処理を返す
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public static Action<MeshRenderer> GetFixTextureSuiteCallbackForSampleSlice(CreateConfig config)
    {
        return (renderer) =>
        {
            var index = config.RandomProviderInstance.Next(0, config.BuildingWallMaterials.Count);

            switch (config.FillGapGroundElement)
            {
                case CreateConfig.FillGapGround.LiftupBuilding:
                    renderer.sharedMaterials = new Material[]
                    {
                        config.BuildingWallSliceBottomMaterials[index],
                        config.BuildingWallMaterials[index],
                        config.BuildingWallSliceTopMaterials[index],
                        config.BuildingFillGapMaterials[config.RandomProviderInstance.Next(0, config.BuildingFillGapMaterials.Count)]
                    };
                    break;
                default:
                    renderer.sharedMaterials = new Material[]
                    {
                        config.BuildingWallSliceBottomMaterials[index],
                        config.BuildingWallMaterials[index],
                        config.BuildingWallSliceTopMaterials[index],
                    };
                    break;
            }
        };
    }

}
}
