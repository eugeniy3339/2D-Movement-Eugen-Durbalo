using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ParalaxeLayer
{
    [Tooltip("A name that you will see (can be empty)")] public string name;
    public Transform layerTransform;
    public float horizontalSpeed;
    public float verticalSpeed;
    public bool multiplyWithLayerStartPosition = true;
    public bool multiplyWithMoveWithStartPosition = true;
    [HideInInspector] public Vector2 layerStartPosition;
}

public enum ParalaxeUpdateMode
{
    LateUpdate,
    Update,
    FixedUpdate
}

public enum ParalaxeMoveWith
{
    Camera,
    Player
}

public class ParalaxeEffect : MonoBehaviour
{
    [SerializeField] private ParalaxeUpdateMode _paralaxeUpdateMode;
    private ParalaxeMoveWith _beforeParalaxeMoveWith;
    [SerializeField] private ParalaxeMoveWith _paralaxeMoveWith;

    [SerializeField] private Camera _camera;
    [SerializeField] private Player _player;

    [SerializeField] private List<ParalaxeLayer> _paralaxeLayers = new List<ParalaxeLayer>();

    private Transform moveWithTransform;
    private Vector2 _moveWithTransformStartPosition;

    private void Awake()
    {
        if(_camera == null) _camera = Camera.main;
        if (_player == null) _player = Player.Instance;

        _beforeParalaxeMoveWith = _paralaxeMoveWith;

        switch (_paralaxeMoveWith)
        {
            case ParalaxeMoveWith.Camera:
                moveWithTransform = _camera.transform;
                break;
            case ParalaxeMoveWith.Player:
                moveWithTransform = _player.transform;
                break;
        }

        _moveWithTransformStartPosition = moveWithTransform.position;

        foreach(var layer in _paralaxeLayers)
        {
            layer.layerStartPosition = layer.layerTransform.position;
        }
    }

    private void LateUpdate()
    {
        if (_paralaxeUpdateMode != ParalaxeUpdateMode.LateUpdate) UpdateParalaxeLayers();
    }

    private void Update()
    {
        if (_paralaxeUpdateMode != ParalaxeUpdateMode.Update) UpdateParalaxeLayers();

        if (_beforeParalaxeMoveWith != _paralaxeMoveWith)
        {
            _beforeParalaxeMoveWith = _paralaxeMoveWith;

            switch (_paralaxeMoveWith)
            {
                case ParalaxeMoveWith.Camera:
                    moveWithTransform = _camera.transform;
                    break;
                case ParalaxeMoveWith.Player:
                    moveWithTransform = _player.transform;
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (_paralaxeUpdateMode != ParalaxeUpdateMode.FixedUpdate) UpdateParalaxeLayers();
    }

    private void UpdateParalaxeLayers()
    {
        foreach (var paralaxeLayer in _paralaxeLayers)
        {
            paralaxeLayer.layerTransform.position = (paralaxeLayer.multiplyWithMoveWithStartPosition ? _moveWithTransformStartPosition : Vector2.zero) + (paralaxeLayer.multiplyWithLayerStartPosition ? paralaxeLayer.layerStartPosition : Vector2.zero) + new Vector2(moveWithTransform.position.x * paralaxeLayer.horizontalSpeed, moveWithTransform.position.y * paralaxeLayer.verticalSpeed);
        }
    }
}