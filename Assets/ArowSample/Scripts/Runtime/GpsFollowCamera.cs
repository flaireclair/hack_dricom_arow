using UnityEngine;

namespace ArowSample.Scripts.Runtime
{
public class GpsFollowCamera : MonoBehaviour
{
    [SerializeField]
    private float TargetDistance = -4.0f;
    [SerializeField]
    private Vector2 CameraPosition = Vector2.zero;
    [SerializeField]
    private GameObject _unityChan;

    void Update()
    {
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
        transform.LookAt(_unityChan.transform);
    }

    void OnGUI()
    {
        if (GUI.RepeatButton(new Rect(0, 0, Screen.width / 2, Screen.height / 16), "+", ZoomButtonStyle))
        {
            CameraPosition = CameraPosition + Vector2.down;
        }

        if (GUI.RepeatButton(new Rect(Screen.width / 2, 0, Screen.width / 2, Screen.height / 16), "-", ZoomButtonStyle))
        {
            CameraPosition = CameraPosition + Vector2.up;
        }
    }

    static GUIStyle _zoomButtonStyle = null;
    static GUIStyle ZoomButtonStyle
    {
        get
        {
            if (_zoomButtonStyle == null)
            {
                _zoomButtonStyle = new GUIStyle(GUI.skin.button);
                _zoomButtonStyle.fontSize = 40;
            }

            return _zoomButtonStyle;
        }
    }
}
}
