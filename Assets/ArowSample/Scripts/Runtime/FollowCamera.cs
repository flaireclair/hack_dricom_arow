using UnityEngine;

namespace ArowSample.Scripts.Runtime
{
public class FollowCamera : MonoBehaviour
{
    public Vector3 InputVector3 = Vector3.zero;

    [SerializeField]
    private float Distance = -4.0f;
    private Transform _unityChan;
    private Vector2 _cameraPos = Vector2.zero;

    void Start()
    {
        _unityChan = GameObject.Find("Walkman_unitychan").transform.Find("LookPos");
        _cameraPos.y = 20;
    }

    void Update()
    {
        Vector3 movedVector3 = InputVector3;

        if (20.0f < Mathf.Abs(movedVector3.x))
        {
            _cameraPos.x += Mathf.Sign(movedVector3.x) * -0.05f;
        }

        if (20.0f < Mathf.Abs(movedVector3.y))
        {
            _cameraPos.y += Mathf.Sign(movedVector3.y) * 0.05f;
        }

        float rad = _cameraPos.x;
        transform.position = _unityChan.transform.rotation * new Vector3(
                                 Distance * Mathf.Sin(rad),
                                 _unityChan.transform.position.y + _cameraPos.y,
                                 Distance * Mathf.Cos(rad)
                             ) + new Vector3(
                                 _unityChan.transform.position.x,
                                 0.0f,
                                 _unityChan.transform.position.z
                             );
        transform.LookAt(_unityChan);
    }

    // Update is called once per frame

    public void CameraReset()
    {
        _cameraPos = Vector2.zero;
    }
}
}