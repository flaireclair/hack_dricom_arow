using UnityEngine;
using UnityEditor;
using ArowMain.Runtime;
using ArowLibrary.ArowDefine;

namespace ArowSample.Scripts.Editor
{
public class CreateSamplePrefabConfigList
{
    /// <summary>
    /// サンプル用のPrefabConfigListを作成する
    /// </summary>
    /// <returns></returns>
    public static PrefabConfigList CreateAssetSampleData()
    {
        var config = ScriptableObject.CreateInstance<PrefabConfigList>();
        config.FilePath = "Assets/StreamingAssets/meguro.arowmap";
        config.LargeObjects.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ArowSample/OtherAssets/ConvertPrefab/oneScaled_P01_A.prefab"));
        config.LargeObjects.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ArowSample/OtherAssets/ConvertPrefab/oneScaled_P02_A.prefab"));
        config.MiddleObjects.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ArowSample/OtherAssets/ConvertPrefab/oneScaled_P04_A.prefab"));
        config.MiddleObjects.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ArowSample/OtherAssets/ConvertPrefab/oneScaled_P04_B.prefab"));
        config.SmallObjects.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ArowSample/OtherAssets/ConvertPrefab/oneScaled_P04_D.prefab"));
        config.SmallObjects.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ArowSample/OtherAssets/ConvertPrefab/oneScaled_P11_A.prefab"));
        config.LargeScale = 60;
        config.MiddleScale = 30;
        config.Scale = 0.3f;
        config.IsSquare = false;
        config.IsUseRandomSeed = false;
        config.RandomSeed = 0;
        config.FillGapGroundElement = CreateConfig.FillGapGround.LiftupBuilding;
        config.FillGapGroundMaterilal = new System.Collections.Generic.List<Material>();
        config.FillGapGroundMaterilal.Add(AssetDatabase.LoadAssetAtPath<Material>("Assets/ArowSample/Resources/Buildings/FillGap/FillGapGray.mat"));
        return config;
    }
}

}
