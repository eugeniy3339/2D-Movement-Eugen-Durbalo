using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/*
 
This script is a module for player movement and wouldnt work without a player, player movement and player inputs manager scripts.
 
 */

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Rigidbody2D))]
public class Dash : PlayerComponent
{
    private Player _player;
    private Rigidbody2D _rigidbody;
    private Animator _animator;

    [Header("Dash Settings")]
    [SerializeField] private float _distance = 5f;
    [SerializeField] private float _dashSpeed = 20f;
    [SerializeField] private float _maxDashTime = 0.3f;
    [SerializeField] private float _dashCooldown = 1f;
    private float _curDashCooldown;
    [Header("Visual Settings")]
    [SerializeField] private Slider _dashCooldownSlider;
    private Transform _gfx;
    [Header("Trail Settings")]
    [SerializeField] private GameObject _playerTrailPrefab;
    [SerializeField] private Sprite _trailSprite;
    [SerializeField] private float _trailLifeTime;

    private void Start()
    {
        _player = Player.Instance;
        _rigidbody = GetComponentInChildren<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();

        _gfx = _player.gfx;
    }

    private void Update()
    {
        _curDashCooldown = Mathf.Clamp(_curDashCooldown + Time.deltaTime, 0f, _dashCooldown);
        if(_dashCooldownSlider != null) _dashCooldownSlider.value = _curDashCooldown;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            DashAction();
        }
    }

    public void DashAction()
    {
        if (!_player.movementScript.canWalk) return;
        if (_curDashCooldown < _dashCooldown) return;

        _player.movementScript.canWalk = false;
        StartCoroutine(DashCoro());
    }

    private IEnumerator DashCoro()
    {
        Vector2 moveInput = _player.movementScript.lastMoveInputValue.normalized;
        Vector2 startPosition = transform.position;

        float dashTime = 0f;

        Vector2 lastTrailSpawnTransform = transform.position;

        //_gfx.transform.up = _playerInputsManager.lastMoveInputXY.normalized;

        _animator.Play("StartDash");

        while (Vector2.Distance(transform.position, startPosition) < _distance)
        {
            _animator.SetBool("Dash", true);
            dashTime += Time.deltaTime;
            _rigidbody.linearVelocity = moveInput * _dashSpeed;

            yield return new WaitForSeconds(Time.deltaTime);

            if (_trailLifeTime != 0f) { if (Vector2.Distance(transform.position, lastTrailSpawnTransform) >= _distance / 4) { StartCoroutine("SpawnTrail"); lastTrailSpawnTransform = transform.position; } }
            if (dashTime >= _maxDashTime) break;
        }

        _rigidbody.linearVelocityY = 0f;
        _player.movementScript.canWalk = true;
        _animator.SetBool("Dash", false);
        //_gfx.transform.up = Vector3.up;
        _curDashCooldown = 0f;
    }

    private IEnumerator SpawnTrail()
    {
        GameObject trail = Instantiate(_playerTrailPrefab, transform.position, _gfx.transform.rotation);
        if(_trailSprite != null) trail.GetComponent<SpriteRenderer>().sprite = _trailSprite;
        else trail.GetComponentInChildren<SpriteRenderer>().sprite = _gfx.GetComponentInChildren<SpriteRenderer>().sprite;
        yield return new WaitForSeconds(_trailLifeTime);
        Destroy(trail);
    }
}
