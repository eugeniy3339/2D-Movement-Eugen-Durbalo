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

    private bool _dashing;

    private void Awake()
    {
        _rigidbody = GetComponentInChildren<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        _player = Player.Instance;
        _gfx = _player.gfx;

        _player.movementScript.onPlayerStateChanged += OnPlayerStateChanged;
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
        if (!_player.movementScript.canWalk || _player.movementScript.playerState != PlayerState.Movement) return;
        if (_curDashCooldown < _dashCooldown) return;

        _player.movementScript.canWalk = false;
        _player.movementScript.playerState = PlayerState.Dashing;
        StartCoroutine(DashCoro());
    }

    private IEnumerator DashCoro()
    {
        Vector2 moveInput = _player.movementScript.lastMoveInputValue.normalized;
        Vector2 startPosition = transform.position;

        float dashTime = 0f;

        Vector2 lastTrailSpawnTransform = transform.position;

        _animator.Play("StartDash");

        _dashing = true;

        while (Vector2.Distance(transform.position, startPosition) < _distance)
        {
            _animator.SetBool("Dash", true);
            dashTime += Time.deltaTime;
            _rigidbody.linearVelocity = moveInput * _dashSpeed;

            yield return new WaitForSeconds(Time.deltaTime);

            if (_trailLifeTime != 0f) { if (Vector2.Distance(transform.position, lastTrailSpawnTransform) >= _distance / 4) { SpawnTrail(); lastTrailSpawnTransform = transform.position; } }
            if (dashTime >= _maxDashTime) break;
        }

        StopDash();
    }

    private void SpawnTrail()
    {
        GameObject trail = Instantiate(_playerTrailPrefab, transform.position, _gfx.transform.rotation);
        if(_trailSprite != null) trail.GetComponent<SpriteRenderer>().sprite = _trailSprite;
        else trail.GetComponentInChildren<SpriteRenderer>().sprite = _gfx.GetComponentInChildren<SpriteRenderer>().sprite;
        Destroy(trail, _trailLifeTime);
    }

    private void StopDash()
    {
        StopCoroutine("DashCoro");

        _rigidbody.linearVelocityY = 0f;
        _animator.SetBool("Dash", false);
        _curDashCooldown = 0f;
        _dashing = false;
        _player.movementScript.curGravityScale = _player.movementScript.normalGravityScale;

        if (_player.movementScript.playerState == PlayerState.Dashing) { _player.movementScript.canWalk = true; _player.movementScript.playerState = PlayerState.Movement; }
        if (_player.movementScript.getOnTheLadderMehode != GetOnTheLadderMethode.Input) _player.movementScript.GetOnLadder();
    }

    private void OnPlayerStateChanged(PlayerState beforeState, PlayerState currentState)
    {
        if (_dashing && currentState == PlayerState.Stunned)
        {
            StopDash();
        }
    }
}
