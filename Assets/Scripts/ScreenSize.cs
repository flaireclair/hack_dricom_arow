using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

/// <summary>
/// アス比に合わせてUIの位置やサイズを調整するクラス
/// </summary>
[ExecuteInEditMode]
public class ScreenSize : MonoBehaviour
{
  //現在のアス比
  private float _currentAspectRate = 0;

  public  float CurrentAspectRate
    {
        get
        {
            return _currentAspectRate;
        }
    }

//=================================================================================
//初期化
//=================================================================================

  protected virtual void Awake()
    {
        AdjustUI ();
    }

  //インスペクターで値が変更された時
  protected virtual void OnValidate()
    {
        AdjustUI();
    }

  //=================================================================================
  //更新
  //=================================================================================

  protected virtual void Update () {
    //アス比が変わったら調整
    if(_currentAspectRate != Screen.width / Screen.height)
        {
            AdjustUI ();
        }
  }

  //UIを調整
  protected virtual void AdjustUI()
    {
        _currentAspectRate = (float)Screen.width / (float)Screen.height;
        Debug.Log ("現在のアス比 : " + _currentAspectRate);
    }

}