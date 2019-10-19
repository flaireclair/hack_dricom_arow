using UnityEngine;

namespace ArowSample.Scripts.Runtime
{
public class UnityChanAnimator : MonoBehaviour
{
    public Vector3 InputVector3 = Vector3.zero;

    private Animator _animator = null;
    private Rigidbody _rigidbody = null;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 movedVector3 = InputVector3;

        if (50.0f < Mathf.Abs(movedVector3.x))
        {
            transform.Rotate(Vector3.up, Mathf.Sign(movedVector3.x) * 2.0f);
        }

        if (30.0f < movedVector3.y)
        {
            _rigidbody.velocity = transform.forward * 7.0f;
        }
        else
        {
            _rigidbody.velocity = _rigidbody.velocity * 0.5f;
        }

        float walkVelocity = 3.0f;

        if (walkVelocity < _rigidbody.velocity.magnitude)
        {
            _animator.SetFloat("Speed", 2.0f);
        }
        else
        {
            _animator.SetFloat("Speed", 0.0f);
        }
    }
}
}
