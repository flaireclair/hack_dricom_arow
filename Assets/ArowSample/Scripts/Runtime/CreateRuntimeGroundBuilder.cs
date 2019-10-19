using ArowLibrary.ArowDefine.SchemaWrapper;
using ArowMain.Runtime.CreateModelScripts;
using UnityEngine;

namespace ArowSample.Scripts.Runtime
{
public class CreateRuntimeGroundBuilder : MonoBehaviour
{
    private GameObject ParentObj;

    public void CreateGround(ArowMapObjectModel arowMapObjectModel)
    {
        CreateConfigGroundMap config = ScriptableObject.CreateInstance<CreateConfigGroundMap>();
        config.RoadDataModels = arowMapObjectModel.RoadDataModels;
        config.IsVisibleColorHeight = false;
        config.MaxHeightContourLine = 30f * 5f;
        config.MinHeightContourLine = -10f;
        config.HeightScale = 5f;
        MeasureProcessTime.Start(MeasureProcessTime.Key.Ground, "CreateGround");
        ParentObj = CreateRuntimeUtility.GetOrCreateParentInfoFromArowMapObjectModel(arowMapObjectModel).gameObject;
        GroundMapCreator.Builder builder =
            new GroundMapCreator.Builder(arowMapObjectModel,
                                         CreateRuntimeUtility.GetOrCreateParentInfoFromArowMapObjectModel(arowMapObjectModel).WorldCenter,
                                         CreateRuntimeUtility.GetOrCreateParentInfoFromArowMapObjectModel(arowMapObjectModel).WorldScale)
        .SetParentTransform(ParentObj.transform)
        .SetConfig(config)
        ;
        builder.Build();
        MeasureProcessTime.Stop();
    }
}
}
