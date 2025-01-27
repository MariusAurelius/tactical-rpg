using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] 
    private Transform target;
    private Vector3 _offset;
    private void Awake()
    {
        _offset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        transform. position = target.position + _offset;
    }

}