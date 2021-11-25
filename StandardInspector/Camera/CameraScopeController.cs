using System.Collections.Generic;
using UnityEngine;

public class CameraScopeController : MonoBehaviour
{
    [Header(Headers.Dependancies)]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Camera _cameraComponent;
    [SerializeField] private List<Transform> _followedObjects;

    [Header(Headers.ScriptSettings)]
    [SerializeField, Range(0, 0.5f)] private float _viewPadding = 0f;
    [SerializeField, Range(0, 10f)] private float _scopeSpeed = 0.1f;
    [SerializeField] private MinMax<float> _cameraSizeMinMax;
    [SerializeField] private Vector2MinMax<float> _cameraPositionMinMax;

    public List<Transform> FollowedObjects
    {
        get => _followedObjects;
        set => _followedObjects = value;
    }

    private bool _canFollow;
    public bool CanFollow
    {
        get => _canFollow;
        set => _canFollow = value;
    }

    private float _cameraInitialZPosition;
    private Vector2 _cameraAvgPosition;
    private Vector2 _cameraAspectPosition;
    private float _cameraOrthoMagnitude;
    private float _targetOrthoSize;
    private float _computedScopeSpeed;

    private void Awake()
    {
        _cameraAvgPosition = Vector2.zero;
        _cameraInitialZPosition = _cameraTransform.position.z;
        _canFollow = true;
    }

    private void FixedUpdate()
    {
        if (_canFollow)
        {
            _cameraAvgPosition = Vector2.zero;
            _cameraOrthoMagnitude = new Vector2(_cameraComponent.orthographicSize * _cameraComponent.aspect, _cameraComponent.orthographicSize).magnitude;
            _cameraAspectPosition = WorldspaceToCameraAspect(_cameraTransform.position);
            _targetOrthoSize = 0f;

            FollowedObjects.ForEach(xx => { 
                _cameraAvgPosition += (Vector2)xx.position;
                _targetOrthoSize = Mathf.Max(_targetOrthoSize, Mathf.Abs(Vector2.Distance(WorldspaceToCameraAspect(xx.position),
                    _cameraAspectPosition)) + (_cameraOrthoMagnitude * _viewPadding));
            });

            _computedScopeSpeed = Time.fixedDeltaTime * (1 + Mathf.Abs(Vector2.Distance(_cameraTransform.position, _cameraAvgPosition))) * _scopeSpeed;

            _cameraAvgPosition /= FollowedObjects.Count;
            _cameraAvgPosition.x = Mathf.Clamp(_cameraAvgPosition.x, _cameraPositionMinMax.x.min, _cameraPositionMinMax.x.max);
            _cameraAvgPosition.y = Mathf.Clamp(_cameraAvgPosition.y, _cameraPositionMinMax.y.min, _cameraPositionMinMax.y.max);

            _cameraTransform.position = Vector3.Lerp(
                _cameraTransform.position,
                new Vector3(_cameraAvgPosition.x, _cameraAvgPosition.y, _cameraInitialZPosition),
                _computedScopeSpeed);

            _cameraComponent.orthographicSize = Mathf.Lerp(
                _cameraComponent.orthographicSize,
                Mathf.Clamp(_targetOrthoSize, _cameraSizeMinMax.min, _cameraSizeMinMax.max),
                _computedScopeSpeed);
        }
    }

    private Vector2 WorldspaceToCameraAspect(Vector2 worldspacePosition)
    {
        return new Vector2(worldspacePosition.x / _cameraComponent.aspect, worldspacePosition.y);
    }
}
