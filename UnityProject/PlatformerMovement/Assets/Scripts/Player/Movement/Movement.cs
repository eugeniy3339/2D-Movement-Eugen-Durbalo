using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine.Windows;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.InputSystem;
using UnityEngine.Experimental.GlobalIllumination;

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

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    private Player _player;
    private Rigidbody2D _rigidbody;
    private Animator _animator;

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

    public float normalGravityScale = 1f;
    public float fallingGravityScale = 2f;

    [Header("Stairs")]
    [SerializeField] private float _maxDistanceFromStairsCenter = 0.2f;
    [SerializeField] private int _onStairsLayer;
    [SerializeField] private float _onStairsDrag = 7f;
    [SerializeField] private float _stairsSpeed;
    private bool _onStairs;
    [HideInInspector] public List<Stairs> curStairsList = new List<Stairs>();
    private Stairs _curStairs;
    [SerializeField] private GetOfTheLadderMethode _getOfTheLadderMethode;

    private void Start()
    {
        _playerStartLayer = gameObject.layer;
        //if (GetComponent<TopDownMovement>()) {print("There is another movement script!!!"); this.enabled = false; }
        _player = Player.Instance;
        _rigidbody = GetComponentInChildren<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();

        _gfx = _player.gfx;

        speed = walkSpeed;
        if (normalGravityScale == Mathf.Infinity) normalGravityScale = _rigidbody.gravityScale;

        switch(_getOfTheLadderMethode)
        {
            case GetOfTheLadderMethode.Direction:
                SetStairsExits();
                break;
        }
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
        if(!isGrounded) SetGravityScale();
    }

    private void Move()
    {
        if (!canWalk) return;

        if(_onStairs)
        {
            StairsMovement();
            return;
        }

        float direction = moveInputValue.x;

        if (!run) _animator.SetFloat("x", Mathf.Abs(direction) * 0.5f);
        else _animator.SetFloat("x", Mathf.Abs(direction));

        if(direction != 0f)
        {
            if (direction > 0f) _gfx.GetComponent<SpriteRenderer>().flipX = false;
            else if (direction < 0f) _gfx.GetComponent<SpriteRenderer>().flipX = true;
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

    private void StairsMovement()
    {
        Vector2 direction = new Vector2(0f, moveInputValue.y).normalized;

        if (Mathf.Abs(direction.y) > 0f) _animator.speed = 1f;
        else _animator.speed = 0f;

        _rigidbody.AddForce(direction * _stairsSpeed * 100f, ForceMode2D.Force);

        switch (_getOfTheLadderMethode)
        {
            case GetOfTheLadderMethode.Direction:
                if (IsGoindToLeaveTheStairs(direction.y))
                    { GetOfTheLadder(); }
                break;
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


                switch (_getOfTheLadderMethode)
                {
                    case GetOfTheLadderMethode.Input:
                        if (Mathf.Abs(moveInputValue.y) < 0.1f && isGrounded)
                            { GetOfTheLadder(); }
                        break;
                }
            }
            if (Mathf.Abs(moveInputValue.y) > 0.1f) {
                lastMoveInputY = moveInputValue.y;

                GetOnLadder();
            }
        }
    }

    private void IsGrounded()
    {
        if (!checkIfIsGrounded) return;
        Debug.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - _halfPlayersHeight - 0.2f));
        isGrounded = Physics2D.Raycast(transform.position, Vector3.down, _halfPlayersHeight + 0.2f, _ground);

        _animator.SetBool("InAir", !isGrounded);

        if (!canWalk) return;

        if (beforeIsGrounded != isGrounded)
        {
            if (isGrounded)
            {
                if (canChangeRigidbodyDamping) _rigidbody.linearDamping = _groundDrag;
                _player.movementScript.SetGravityScale(false);
            }
            else
            {
                if (canChangeRigidbodyDamping) _rigidbody.linearDamping = 0f;
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
        if (!canWalk) return;

        if(_onStairs)
        {
            float flatVel = _rigidbody.linearVelocity.y;

            if (Mathf.Abs(flatVel) > speed)
            {
                float limitedVelocity = _rigidbody.linearVelocity.normalized.y * speed;
                _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, limitedVelocity); 
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
                float limitedVelocity = _rigidbody.linearVelocity.normalized.x * speed;
                _rigidbody.linearVelocity = new Vector2(limitedVelocity, _rigidbody.linearVelocity.y);
            }
        }
    }

    private void useGravity(bool use)
    {
        if (!canChangeGravityScale) return;
        float gravityScale = _rigidbody.gravityScale;

        if (use) _rigidbody.gravityScale = 1f;
        else _rigidbody.gravityScale = 0f;

        //if(_rb.gravityScale != gravityScale) _rb.linearVelocity = Vector2.zero;
    }

    private void SetGravityScale()
    {
        if (_rigidbody.linearVelocity.y < 0f)
        {
            SetGravityScale(true);
        }
        else
        {
            SetGravityScale(false);
        }
    }

    public void SetGravityScale(bool fallingGravityScale)
    {
        if (!canChangeGravityScale) return;
        _rigidbody.gravityScale = fallingGravityScale ? this.fallingGravityScale : normalGravityScale;
    }

    private Stairs GetCurrentStairs()
    {
        Stairs closestStairs = null;
        float closestStairsDistance = _maxDistanceFromStairsCenter;
        foreach (var stairs in curStairsList)
        {
            float distance = Vector2.Distance(new Vector2(transform.position.x, 0f), new Vector2(stairs.transform.position.x, 0f));
            if(distance < closestStairsDistance)
            {
                closestStairsDistance = distance;
                closestStairs = stairs;
            }
        }

        return closestStairs;
    }

    private void GetOnLadder()
    {
        if (Mathf.Abs(moveInputValue.x) < 0.1f && !_onStairs && canWalk)
        {
            _curStairs = GetCurrentStairs();

            if (_curStairs)
            {
                gameObject.layer = _onStairsLayer;
                _onStairs = true;
                useGravity(false);
                _rigidbody.linearDamping = _onStairsDrag;
                canChangeRigidbodyDamping = false;
                canChangeGravityScale = false;
                _animator.SetBool("Climbing", true);

                foreach (var playerComponent in _player.components)
                {
                    playerComponent.enabled = false;
                }
            }
        }
    }

    private void GetOfTheLadder()
    {
        if (_onStairs && canWalk)
        {
            _curStairs = null;

            gameObject.layer = _playerStartLayer;
            _onStairs = false;
            useGravity(true);
            _rigidbody.linearDamping = isGrounded ? _groundDrag : 0f;
            canChangeRigidbodyDamping = true;
            canChangeGravityScale = true;
            _animator.SetBool("Climbing", false);
            _animator.speed = 1f;

            foreach (var playerComponent in _player.components)
            {
                playerComponent.enabled = true;
            }
        }
    }

    private bool IsGoindToLeaveTheStairs(float moveDirection)
    {
        if (!_curStairs) return false;

        foreach (var stairsExit in _curStairs.stairsExits)
        {
            float exitDirection = (new Vector2(0f, stairsExit.position.y) - new Vector2(0f, _curStairs.transform.position.y)).normalized.y;

            if(exitDirection == moveDirection && Mathf.Abs(transform.position.y - stairsExit.position.y) <= 0.1f)
            {
                return true;
            }
        }

        return false;
    }

    private void SetStairsExits()
    {
        foreach (var stairs in FindObjectsOfType<Stairs>())
        {
            if (stairs.stairsExits.Count == 0)
            {
                GameObject stairsExitTransform = new GameObject("StairsExit");
                stairsExitTransform.transform.position = new Vector2(stairs.transform.position.x, stairs.transform.position.y + stairs.transform.localScale.y / 2f - _halfPlayersHeight - 0.1f);
                stairsExitTransform.transform.SetParent(stairs.transform, true);

                stairs.stairsExits.Add(stairsExitTransform.transform);

                GameObject stairsExitTransform2 = new GameObject("StairsExit");
                stairsExitTransform2.transform.position = new Vector2(stairs.transform.position.x, stairs.transform.position.y - stairs.transform.localScale.y / 2f + _halfPlayersHeight + 0.1f);
                stairsExitTransform2.transform.SetParent(stairs.transform, true);

                stairs.stairsExits.Add(stairsExitTransform2.transform);
            }
        }
    }
}
