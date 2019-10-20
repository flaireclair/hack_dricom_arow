// UI 関連の記述を簡略化できる
using UnityEngine.UI;
using UnityEngine;

public class ScrollBar : MonoBehaviour
{
    [SerializeField] ScrollRect scrollRect = null;
    [SerializeField] Scrollbar scrollbar = null;

    void Start()
    {// Vertical Scrollbar を設定
        scrollRect.verticalScrollbar = scrollbar;
    }
}