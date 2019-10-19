using ArowMain.Runtime;
using UnityEditor;
using UnityEngine;
using ArowLibrary.ArowDefine;

namespace ArowSample.Scripts.Editor
{
public static class CreateSampleCategoryPoiConfig
{
    /// <summary>
    /// サンプル用のPoiConfigを作成する
    /// </summary>
    /// <returns></returns>
    public static CategoryPoiConfig CreateAssetSampleData()
    {
        var config = ScriptableObject.CreateInstance<CategoryPoiConfig>();
        config.List = new System.Collections.Generic.List<CategoryPoiConfig.CategoryPoi>();
        int index = 0;

        foreach (var i in POIDefine.CategoryTbl.Keys)
        {
            config.List.Add(new CategoryPoiConfig.CategoryPoi(i));

            if (!POIDefine.CategoryTbl.ContainsKey(i))
            {
                index++;
                continue;
            }

            int sub_index = 0;

            foreach (var j in POIDefine.CategoryTbl[i].arowSubCategoryList)
            {
                config.List[index].subcategoryList.Add(new CategoryPoiConfig.SubCategoryPoi(j));

                // 「商業施設」「ファストフード」にのみ、prefabを指定
                if (j == POIDefine.SUBCATEGORY.FAST_FOOD)
                {
                    int fast_food_index = config.List[index].subcategoryList.Count - 1;
                    config.List[index].subcategoryList[fast_food_index].poiPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ArowSample/OtherAssets/ConvertPrefab/oneScaled_P09_A.prefab");
                }

                if (!POIDefine.SubcategoryTbl.ContainsKey(j))
                {
                    sub_index++;
                    continue;
                }

                foreach (var k in POIDefine.SubcategoryTbl[j].arowSubsubCategoryList)
                {
                    config.List[index].subcategoryList[sub_index].subsubcategoryList.Add(new CategoryPoiConfig.SubSubCategoryPoi(k));
                }

                sub_index++;
            }

            index++;
        }

        return config;
    }
}
}
