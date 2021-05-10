using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target = default;
    [SerializeField] private Vector3 _offset = default;
    [SerializeField] private float _smoothDampValue = 0.125f;
    private Vector3 _smoothDampVelocity;

    void FixedUpdate()
    {
        Vector3 desiredPosition = _target.position + _offset;
        //Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref _smoothDampVelocity, _smoothDampValue);
        transform.position = desiredPosition;
    }
}
