using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

public class CameraScopeController : MonoBehaviour
{
    [Title(Headers.Dependancies)]
    [SerializeField, OdinSerialize] private Transform _cameraTransform;
    [SerializeField, OdinSerialize] private Camera _cameraComponent;
    [SerializeField, OdinSerialize] private List<Transform> _followedObjects;

    [Title(Headers.ScriptSettings)]
    [SerializeField, OdinSerialize, Range(0, 0.5f)] private float _viewPadding = 0f;
    [SerializeField, OdinSerialize, Range(0, 10f)] private float _scopeSpeed = 0.1f;
    [Space]
    [SerializeField, OdinSerialize] private Vector2 _cameraPositionOffset;
    [SerializeField, OdinSerialize] private InspectorTogglable<MinMax<float>> _cameraSizeMinMax;
    [SerializeField, OdinSerialize] private InspectorTogglable<Vector2MinMax<float>> _cameraPositionMinMax;

    private bool _canFollow;
    private Vector2 _cameraAvgPosition;
    private Vector2 _cameraAspectPosition;
    private float _cameraInitialZPosition;
    private float _cameraOrthoMagnitude;
    private float _targetOrthoSize;
    private float _computedScopeSpeed;

    public List<Transform> FollowedObjects
    {
        get => _followedObjects;
        set => _followedObjects = value;
    }

    public bool CanFollow
    {
        get => _canFollow;
        set => _canFollow = value;
    }

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

            if (_cameraPositionMinMax.IsEnabled) 
            {
                _cameraAvgPosition /= FollowedObjects.Count;
                _cameraAvgPosition.x = Mathf.Clamp(_cameraAvgPosition.x, _cameraPositionMinMax.Value.x.min, _cameraPositionMinMax.Value.x.max);
                _cameraAvgPosition.y = Mathf.Clamp(_cameraAvgPosition.y, _cameraPositionMinMax.Value.y.min, _cameraPositionMinMax.Value.y.max);
            }

            if (_cameraSizeMinMax.IsEnabled)
                _targetOrthoSize = Mathf.Clamp(_targetOrthoSize, _cameraSizeMinMax.Value.min, _cameraSizeMinMax.Value.max);

            _cameraTransform.position = Vector3.Lerp(
                _cameraTransform.position,
                new Vector3(_cameraAvgPosition.x, _cameraAvgPosition.y, _cameraInitialZPosition),
                _computedScopeSpeed);

            _cameraComponent.orthographicSize = Mathf.Lerp(
                _cameraComponent.orthographicSize,
                _targetOrthoSize,
                _computedScopeSpeed);
        }
    }

    private Vector2 WorldspaceToCameraAspect(Vector2 worldspacePosition)
    {
        return new Vector2(worldspacePosition.x / _cameraComponent.aspect, worldspacePosition.y);
    }
}
