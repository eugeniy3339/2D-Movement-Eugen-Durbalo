using UnityEngine;

/*

To download newest original version go to the https://github.com/eugeniy3339/2D-Movement-Eugen-Durbalo/tree/main page!
 
UA

Це скрипт для 2Д Мувменту з видом з верху. Ви можете налаштовувати змінні так, як ви хочете. Також ви можете додавати до гравця такі компоненти як {Run, Dash...}, які знаходятся у теці Components. (Компонент Jump не буде працювати в цьому режимі)
Також цей скрипт потребує щоб на гейм об'єкті гравця знаходився Player скрипт, PlayerInputsManager скрипт та RigidBody (також бажано мати колайдер).
Якщо у вас немає якихось специфічних потріб, раджу не чипати змінну _groundDrag.

(Нагадую що ви можете змінювати кожний скрипт під свої потреби (https://en.wikipedia.org/wiki/MIT_License))

EN

This is script for 2D TopDown Movement. You can set variables by your needs. Also you can add components like {Run, Dash...}, wich are in the Components Folder. (Jump component woldnt work with this script)
Also this script require Player script, PlayerInputsManager script and Rigidbody be on gameObject (also it will be better with collider).
If you havent some specific needs, it will be better for you not to change variable _groundDrag.

(You can also change all the scripts if you want to (https://en.wikipedia.org/wiki/MIT_License))

SK

Toto je skript pre 2D Platformer Mopvement. Môžete si upraviť premenné podľa vlastných potrieb. Taktiež môžete pridať komponenty ako {Run, Dash...} ktoré sa nachádzajú v priečinku Components. (Jump komponent nebude fungovat)
Pre správne fungovanie skriptu je potrebné, aby mal gameObject skripty: Player, PlayerInputsManager a komponent Rigidbody (odporúča sa aj nejaký Collider).
Ak nemáte špecifické požiadavky, odporúčam nemeníť hodnoty premennu _groundDrag.

(Skript (ako aj ostatné) môžete slobodne upravovať podľa potreby (https://en.wikipedia.org/wiki/MIT_License))
 
 */

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerInputsManager))]
[RequireComponent(typeof(Rigidbody))]
public class TopDownMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    private PlayerInputsManager _inputs;

    private Transform _gfx;

    [Header("Movement Settings/Movement")]
    [Tooltip("Default player speed")]public float walkSpeed = 1f;
    [Tooltip("Player run speed. Works only with a run component (If you dont have one on this gameObject change Walk Speed variable)")] public float runSpeed = 2f;
    [Tooltip("A ground drag (change only if you know what you are doing!!!)")][SerializeField] private float _groundDrag = 5f;

    [HideInInspector] public float speed;
    [HideInInspector] public bool canWalk = true;
    [HideInInspector] public bool run;

    private void Awake()
    {
        //if (GetComponent<TopDownMovement>()) {print("There is another movement script!!!"); this.enabled = false; }
        _rb = GetComponent<Rigidbody2D>();
        _inputs = GetComponent<PlayerInputsManager>();

        _rb.linearDamping = _groundDrag;
        _gfx = GetComponent<Player>().gfx;

        speed = walkSpeed;
    }

    private void Update()
    {
        SpeedControl();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        if (!canWalk) return;

        Vector2 direction = _inputs.moveInputValue;

        /*if (!run) _animator.SetFloat("MoveInputValue", direction.magnitude * 0.5f, _animSmoothTime, Time.deltaTime);
        else _animator.SetFloat("MoveInputValue", direction.magnitude, _animSmoothTime, Time.deltaTime);*/

        if (_inputs.lastMoveInputX > 0f) _gfx.GetComponent<SpriteRenderer>().flipX = false;
        else if (_inputs.lastMoveInputX < 0f) _gfx.GetComponent<SpriteRenderer>().flipX = true;

        if (direction.magnitude < 0.1f) return;

        Vector2 moveDir = direction;

        Debug.DrawRay(transform.position, moveDir.normalized * 10f, Color.green);
        _rb.AddForce(moveDir.normalized * speed * 100f, ForceMode2D.Force);
    }

    private void SpeedControl()
    {
        if (!canWalk) return;

        if (_rb.linearVelocity.magnitude > speed)
        {
            _rb.linearVelocity = _rb.linearVelocity.normalized * speed;
        }
    }
}
