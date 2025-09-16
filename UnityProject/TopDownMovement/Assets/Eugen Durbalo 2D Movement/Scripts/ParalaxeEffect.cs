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
    [SerializeField] private ParalaxeMoveWith _paralaxeMoveWith;

    [SerializeField] private Camera _camera;
    [SerializeField] private Player _player;

    [SerializeField] private List<ParalaxeLayer> _paralaxeLayers = new List<ParalaxeLayer>();

    private Transform moveWithTransform;

    private void Awake()
    {
        if(_camera == null) _camera = Camera.main;
        if (_player == null) _player = Player.Instance;

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

    private void LateUpdate()
    {
        switch (_paralaxeUpdateMode)
        {
            case ParalaxeUpdateMode.LateUpdate:
                UpdateParalaxeLayers();
                break;
        }
    }

    private void Update()
    {
        switch (_paralaxeUpdateMode)
        {
            case ParalaxeUpdateMode.Update:
                UpdateParalaxeLayers();
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (_paralaxeUpdateMode)
        {
            case ParalaxeUpdateMode.FixedUpdate:
                UpdateParalaxeLayers();
                break;
        }
    }

    private void UpdateParalaxeLayers()
    {
        foreach (var paralaxeLayer in _paralaxeLayers)
        {
            paralaxeLayer.layerTransform.position = new Vector2(moveWithTransform.position.x * paralaxeLayer.horizontalSpeed, moveWithTransform.position.x * paralaxeLayer.verticalSpeed);
        }
    }
}