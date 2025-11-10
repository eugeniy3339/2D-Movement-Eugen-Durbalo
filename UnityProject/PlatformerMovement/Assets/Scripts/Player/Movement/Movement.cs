using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*

To download newest original version go to the https://github.com/eugeniy3339/2D-Movement-Eugen-Durbalo/tree/main page!
 
UA

Це скрипт для 2Д Мувменту з видом з боку. Ви можете налаштовувати змінні так, як ви хочете. Також ви можете додавати до гравця такі компоненти як {Run, Jump, Dash...}, які знаходятся у теці Components.
Також цей скрипт потребує щоб на гейм об'єкті гравця знаходився Player скрипт, PlayerInputsManager скрипт та RigidBody (також бажано мати колайдер).
Якщо у вас немає якихось специфічних потріб, раджу не чипати змінні _groundDrag та _airMultiplier.

(Нагадую що ви можете змінювати кожний скрипт під свої потреби (https://en.wikipedia.org/wiki/MIT_License))

EN

This is script for 2D Platformer Movement. You can set variables by your needs. Also you can add components like {Run, Jump, Dash...}, wich are in the Components Folder.
Also this script require Player script, PlayerInputsManager script and Rigidbody be on gameObject (also it will be better with collider).
If you havent some specific needs, it will be better for you not to change variables _groundDrag and _airMultiplier.

(You can also change all the scripts if you want to (https://en.wikipedia.org/wiki/MIT_License))

SK

Toto je skript pre 2D Platformer Mopvement. Môžete si upraviť premenné podľa vlastných potrieb. Taktiež môžete pridať komponenty ako {Run, Jump, Dash...} ktoré sa nachádzajú v priečinku Components.
Pre správne fungovanie skriptu je potrebné, aby mal gameObject skripty: Player, PlayerInputsManager a komponent Rigidbody (odporúča sa aj nejaký Collider).
Ak nemáte špecifické požiadavky, odporúčam nemeníť hodnoty premenných _groundDrag a _airMultiplier.

(Skript (ako aj ostatné) môžete slobodne upravovať podľa potreby (https://en.wikipedia.org/wiki/MIT_License))
 
 */

public enum GetOfTheLadderMethode
{
    Input,
    Direction
}

public enum PlayerState
{
    Movement,
    Dashing,
    Stunned,
    Attacking
}

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    private Player _player;
    private Rigidbody2D _rigidbody;
    private Animator _animator;

    private PlayerState _playerState;
    public PlayerState playerState 
    {
        get { 
            return _playerState; 
        }
        set {
            if (playerState != value) { 
                _beforePlayerState = _playerState;
                _playerState = value;
                if(onPlayerStateChanged != null)
                {
                    onPlayerStateChanged(_beforePlayerState, value);
                }
            } 
        }
    }
    private PlayerState _beforePlayerState;

    public delegate void OnPlayerStateChanged(PlayerState beforeState, PlayerState newState);
    public event OnPlayerStateChanged onPlayerStateChanged;

    private Transform _gfx;

    [Header("Movement Settings/Ground Checking")]
    [SerializeField] private LayerMask _ground;
    [SerializeField] private float _halfPlayersHeight = 1f;
    [Header("Movement Settings/Movement")]
    public float walkSpeed = 2f;
    [SerializeField] private float _airMultiplier = 0.8f;
    [SerializeField] private float _groundDrag = 5f;
    public float groundDrag { get { return _groundDrag; } }
    [HideInInspector] public Vector2 moveInputValue;
    [HideInInspector] public Vector2 lastMoveInputValue;
    [HideInInspector] public float lastMoveInputX;
    [HideInInspector] public float lastMoveInputY;
    [Header("Movement/Slopes")]
    [SerializeField] private float _maxSlopeAngle = 40f;
    private RaycastHit2D _slopeHit;

    private int _playerStartLayer;
    [HideInInspector] public float speed;
    [HideInInspector] public bool canWalk = true;
    [HideInInspector] public bool canChangeRigidbodyDamping = true;
    [HideInInspector] public bool canChangeGravityScale = true;
    [HideInInspector] public bool slopesSpeedControl = true;
    [HideInInspector] public bool checkIfIsGrounded = true;
    [HideInInspector] public bool run;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool beforeIsGrounded;
    [Header("Gravity")]
    public float normalGravityScale = 1f;
    [SerializeField] private float _maxFallSpeed = 50f;
    [Header("Lader")]
    [SerializeField] private float _onLaderDrag = 15f;
    [SerializeField] private float _onLaderSpeed = 3.5f;
    private List<Lader> _curLaders = new List<Lader>();
    public List<Lader> curLaders { get { return _curLaders; } }
    private bool _onLader;
    public bool onLader { get { return _onLader; } }
    public event Action onGetOnLader;

    private float _curGravityScale;
    public float curGravityScale
    {
        set {
            if (canChangeGravityScale)
            {
                _curGravityScale = value;
                gravityScale = value;
            }
        }
        get {
            return _curGravityScale;
        }
    }

    private float _gravityScale;
    public float gravityScale
    {
        get
        {
            return _gravityScale;
        }
        set
        {
            if (canChangeGravityScale)
            {
                _gravityScale = value;
                _rigidbody.gravityScale = value;
            }
        }
    }

    private float _curDrag;
    public float curDrag {
        get { 
            return _curDrag; 
        }
        set {
            if(canChangeRigidbodyDamping)
            {
                _curDrag = value;
                _rigidbody.linearDamping = value;
            }
        }
    }

    private void Awake()
    {
        _rigidbody = GetComponentInChildren<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();

        _playerStartLayer = gameObject.layer;

        playerState = PlayerState.Movement;
        canChangeGravityScale = true;
        speed = walkSpeed;

        if (normalGravityScale == Mathf.Infinity) normalGravityScale = _rigidbody.gravityScale;
        curGravityScale = normalGravityScale;
    }

    private void Start()
    {
        _player = Player.Instance;
        _gfx = _player.gfx;

        
    }

    private void Update()
    {
        SpeedControl();
        IsGrounded();

        useGravity(!OnSlope());
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (!canWalk || (playerState == PlayerState.Dashing || playerState == PlayerState.Stunned)) return;

        if(onLader)
        {
            LaderMovement();
            return;
        }

        float direction = moveInputValue.x;

        if (!run) _animator.SetFloat("x", Mathf.Abs(direction) * 0.5f);
        else _animator.SetFloat("x", Mathf.Abs(direction));

        if(direction != 0f)
        {
            if (direction > 0f) _player.gfxFlipX = false;
            else if (direction < 0f) _player.gfxFlipX = true;
        }

        if (Mathf.Abs(direction) < 0.1f) return;

        Vector2 moveDir = new Vector2(direction, 0f).normalized;

        if (OnSlope())
        {
            moveDir = GetSlopeMoveDirection(moveDir);
            moveDir *= 8f;
        }

        Debug.DrawRay(transform.position, moveDir.normalized * 10f, Color.green);
        if (isGrounded) _rigidbody.AddForce(moveDir.normalized * speed * 100f, ForceMode2D.Force);
        else _rigidbody.AddForce(moveDir.normalized * speed * 100f * _airMultiplier, ForceMode2D.Force);
    }

    private void LaderMovement()
    {
        if (moveInputValue.magnitude < 0.1f)
            _animator.speed = 0f;
        else
        {
            _animator.speed = 1f;
            _rigidbody.AddForce(moveInputValue.normalized * _onLaderSpeed * 100f, ForceMode2D.Force);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInputValue = context.ReadValue<Vector2>();
        if(moveInputValue.magnitude > 0.1f)
        {
            lastMoveInputValue = moveInputValue;
            if (Mathf.Abs(moveInputValue.x) > 0.1f) {
                lastMoveInputX = moveInputValue.x;
            }
            if (Mathf.Abs(moveInputValue.y) > 0.1f) {
                lastMoveInputY = moveInputValue.y;

                GetOnLader();
            }
        }
    }

    private void IsGrounded()
    {
        if (!checkIfIsGrounded) return;
        Debug.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - _halfPlayersHeight - 0.2f));
        isGrounded = Physics2D.Raycast(transform.position, Vector3.down, _halfPlayersHeight + 0.2f, _ground);

        _animator.SetBool("InAir", !isGrounded);

        if (beforeIsGrounded != isGrounded)
        {
            if (isGrounded)
            {
                curDrag = _groundDrag;
                curGravityScale = normalGravityScale;
            }
            else
            {
                curDrag = 0f;
            }

            beforeIsGrounded = isGrounded;
        }
    }

    public bool OnSlope()
    {
        _slopeHit = Physics2D.Raycast(transform.position, Vector2.down, _halfPlayersHeight + 0.2f, _ground);

        if (_slopeHit == true)
        {
            float angle = Vector2.Angle(Vector2.up, _slopeHit.normal);
            return angle < _maxSlopeAngle && angle != 0f;
        }

        return false;
    }

    public Vector2 GetSlopeMoveDirection(Vector2 normal)
    {
        Vector3 dir = Vector3.ProjectOnPlane(normal, _slopeHit.normal).normalized;

        return new Vector2(dir.x, dir.y);
    }

    private void SpeedControl()
    {
        if(playerState != PlayerState.Dashing)
        {
            float flatVel = -_rigidbody.linearVelocity.y;
            if(flatVel > _maxFallSpeed)
            {
                float limitedVelocity = -_maxFallSpeed;
                _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, limitedVelocity);
            }
        }

        if (!canWalk) return;

        if(onLader)
        {
            if (_rigidbody.linearVelocity.magnitude > _onLaderSpeed)
            {
                _rigidbody.linearVelocity = _rigidbody.linearVelocity.normalized * _onLaderSpeed;
            }
            return;
        }

        if (OnSlope())
        {
            if(!slopesSpeedControl) return;

            if (_rigidbody.linearVelocity.magnitude > speed)
            {
                _rigidbody.linearVelocity = _rigidbody.linearVelocity.normalized * speed;
            }
        }
        else
        {
            float flatVel = _rigidbody.linearVelocity.x;

            if (Mathf.Abs(flatVel) > speed)
            {
                float limitedVelocity = (_rigidbody.linearVelocity.normalized.x > 0f ? 1f : -1f) * speed;
                _rigidbody.linearVelocity = new Vector2(limitedVelocity, _rigidbody.linearVelocity.y);
            }
        }
    }

    private void useGravity(bool use)
    {
        if (use) gravityScale = curGravityScale;
        else gravityScale = 0f;
    }



    //Stairs
    public void GetOnLader()
    {
        if(playerState == PlayerState.Movement && IsGoingToGetOnLader())
        {
            if(onGetOnLader != null)
            {
                onGetOnLader();
            }
            _onLader = true;
            useGravity(false);
            curDrag = _onLaderDrag;
            canChangeGravityScale = false;
            canChangeRigidbodyDamping = false;
            _animator.SetBool("Climbing", true);
            _animator.Play("Climbing");
        }
    }

    public void GetOfLader()
    {
        if(_onLader)
        {
            _onLader = false;
            canChangeGravityScale = true;
            canChangeRigidbodyDamping = true;
            useGravity(true);
            curDrag = isGrounded ? _groundDrag : 0f;
            _animator.SetBool("Climbing", false);
            _animator.speed = 1f;
        }
    }

    private bool IsGoingToGetOnLader()
    {
        return _curLaders.Count > 0;
    }

    public void AddLader(Lader stairs)
    {
        _curLaders.Add(stairs);
    }

    public bool RemoveLader(Lader stairs)
    {
        bool removed = _curLaders.Remove(stairs);
        if (_curLaders.Count == 0)
            GetOfLader();
        return removed;
    }
}