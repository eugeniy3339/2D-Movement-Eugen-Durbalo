using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine.Windows;

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


[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerInputsManager))]
[RequireComponent(typeof(Rigidbody))]
public class PlatformerMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    private PlayerInputsManager _inputs;
    private Animator _animator;

    private Transform _gfx;

    [Header("Movement Settings/Ground Checking")]
    [SerializeField] private LayerMask _ground;
    [SerializeField] private float _halfPlayersHeight = 1f;
    [Header("Movement Settings/Movement")]
    public float walkSpeed = 1f;
    public float runSpeed = 2f;
    [SerializeField] private float _airMultiplier = 0.4f;
    [SerializeField] private float _groundDrag = 5f;
    [Header("Movement/Slopes")]
    [SerializeField] private float _maxSlopeAngle = 40f;
    private RaycastHit2D _slopeHit;
    [HideInInspector] public float speed;
    [HideInInspector] public bool canWalk = true;
    [HideInInspector] public bool slopesSpeedControl = true;
    [HideInInspector] public bool run;
    [HideInInspector] public bool isGrounded;

    private void Awake()
    {
        //if (GetComponent<TopDownMovement>()) {print("There is another movement script!!!"); this.enabled = false; }
        _rb = GetComponentInChildren<Rigidbody2D>();
        _inputs = GetComponentInChildren<PlayerInputsManager>();
        _animator = GetComponentInChildren<Animator>();

        _gfx = GetComponentInChildren<Player>().gfx;

        speed = walkSpeed;
    }

    private void Update()
    {
        SpeedControl();
        Gravitation();

        useGravity(!OnSlope());
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        if (!canWalk) return;

        float direction = _inputs.moveInputValue.x;

        if (!run) _animator.SetFloat("x", Mathf.Abs(direction) * 0.5f);
        else _animator.SetFloat("x", Mathf.Abs(direction));

        if(_inputs.lastMoveInputX > 0f) _gfx.GetComponent<SpriteRenderer>().flipX = false;
        else if(_inputs.lastMoveInputX < 0f) _gfx.GetComponent<SpriteRenderer>().flipX = true;

        if (Mathf.Abs(direction) < 0.1f) return;

        Vector2 moveDir = new Vector2(1f, 0f) * direction;

        if (OnSlope())
        {
            moveDir = GetSlopeMoveDirection(moveDir);
            moveDir *= 8f;
        }

        Debug.DrawRay(transform.position, moveDir.normalized * 10f, Color.green);
        if (isGrounded) _rb.AddForce(moveDir.normalized * speed * 100f, ForceMode2D.Force);
        else _rb.AddForce(moveDir.normalized * speed * 100f * _airMultiplier, ForceMode2D.Force);
    }

    private void Gravitation()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector3.down, _halfPlayersHeight + 0.1f, _ground);

        _animator.SetBool("InAir", !isGrounded);

        if (!canWalk) return;

        if (isGrounded) _rb.linearDamping = _groundDrag;
        else _rb.linearDamping = 0f;
    }

    private bool OnSlope()
    {
        _slopeHit = Physics2D.Raycast(transform.position, Vector2.down, _halfPlayersHeight + 0.1f, _ground);

        if (_slopeHit == true)
        {
            float angle = Vector2.Angle(Vector2.up, _slopeHit.normal);
            return angle < _maxSlopeAngle && angle != 0f;
        }

        return false;
    }

    private Vector2 GetSlopeMoveDirection(Vector2 normal)
    {
        Vector3 dir = Vector3.ProjectOnPlane(normal, _slopeHit.normal).normalized;

        return new Vector2(dir.x, dir.y);
    }

    private void SpeedControl()
    {
        if (!canWalk) return;

        if (OnSlope())
        {
            if(!slopesSpeedControl) return;

            if (_rb.linearVelocity.magnitude > speed)
            {
                _rb.linearVelocity = _rb.linearVelocity.normalized * speed;
            }
        }
        else
        {
            float flatVel = _rb.linearVelocity.x;

            if (Mathf.Abs(flatVel) > speed)
            {
                float limitedVelocity = _rb.linearVelocity.normalized.x * speed;
                _rb.linearVelocity = new Vector2(limitedVelocity, _rb.linearVelocity.y);
            }
        }
    }

    private void useGravity(bool use)
    {
        float gravityScale = _rb.gravityScale;

        if (use) _rb.gravityScale = 1f;
        else _rb.gravityScale = 0f;

        //if(_rb.gravityScale != gravityScale) _rb.linearVelocity = Vector2.zero;
    }
}
