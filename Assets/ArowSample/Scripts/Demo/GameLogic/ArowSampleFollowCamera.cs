using UnityEngine;

namespace ArowSampleGame.SampleScripts
{
/// <summary>
/// サンプルシーン用のカメラ制御クラス
/// </summary>
public class ArowSampleFollowCamera : MonoBehaviour
{
    // カメラ距離用
    const float DISTANCE_MAX = -1.0f;	// カメラが離れられる最大距離
    const float DISTANCE_MIN = -10.0f;	// カメラが近づける最小距離

    const float VERTICAL_SPEED = 0.1f;	// 縦方向への移動スピード

    [SerializeField]
    private float TargetDistance = -4.0f;
    [SerializeField]
    private Vector2 CameraPosition = Vector2.zero;
    private Transform _unityChan;

    void Start()
    {
        _unityChan = GameObject.Find("Walkman_unitychan_sample").transform.Find("LookPos");
    }

    void Update()
    {
#if UNITY_EDITOR
        CameraPosition.y += Input.GetAxis("Mouse Y") * VERTICAL_SPEED;
        var sd = Input.mouseScrollDelta;

        if (sd.y * sd.y > 0.5f)
        {
            TargetDistance += sd.y;
            TargetDistance = Mathf.Clamp(TargetDistance, DISTANCE_MIN, DISTANCE_MAX);
        }

#endif
        float rad = CameraPosition.x;
        transform.position = _unityChan.transform.rotation * new Vector3(
                                 TargetDistance * Mathf.Sin(rad),
                                 _unityChan.transform.position.y + CameraPosition.y,
                                 TargetDistance * Mathf.Cos(rad)
                             ) + new Vector3(
                                 _unityChan.transform.position.x,
                                 0.0f,
                                 _unityChan.transform.position.z
                             );
        transform.LookAt(_unityChan);
    }
}
}