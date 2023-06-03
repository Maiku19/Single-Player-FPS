using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    [SerializeField] Transform _movementDirectionReference;
    [SerializeField] float _speed = 1;
    [SerializeField] float _acceleration = 1;
    [SerializeField] float _deceleration = 1;
    public float jumpHeight = 2f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groudMask;
    [SerializeField, InspectorName("Jump Input Time Tolerance")] float _jumpInputTime = 0.25f;

    bool _isGrounded;
    Rigidbody _rb;
    float _timer = 0;

    [SerializeField] float _gravity = -9.81f;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.velocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        DoGroundCheck();
        Move();
        Gravity(Vector3.up * _gravity);
    }

    private void Update()
    {
        Jump();
    }

    void Gravity(Vector3 gravity)
    {
        _rb.AddForce((1 + _rb.drag) * _rb.mass * gravity, ForceMode.Force);
    }

    private void Move()
    {
        Vector3 dir = KeyBinds.Movement.x * _movementDirectionReference.right + KeyBinds.Movement.y * _movementDirectionReference.forward;

        _rb.AddForce(_acceleration * Time.fixedDeltaTime * _rb.drag * _rb.mass * 1000 * dir, ForceMode.Force);
        _rb.AddForce(_deceleration * Time.fixedDeltaTime * _rb.drag * _rb.mass * 1000 * -new Vector3(dir.x == 0 ? math.clamp(_rb.velocity.x, -1 ,1) : 0, 0, dir.z == 0 ? math.clamp(_rb.velocity.z, -1, 1) : 0), ForceMode.Force);

        _rb.velocity = Vector3.ClampMagnitude(mul(_rb.velocity, new(1,0,1)), _speed) + Vector3.up * _rb.velocity.y;

        //Vector3 aprx = new(math.abs(_rb.velocity.x) - .1f <= 0 ? 0 : _rb.velocity.x, _rb.velocity.y, math.abs(_rb.velocity.z) - .1f <= 0 ? 0 : _rb.velocity.z);
        //_rb.velocity = aprx;

        static Vector3 mul(Vector3 a, Vector3 b)
        {
            return new(a.x * b.x, a.y * b.y, a.z * b.z);
        }
    }

    private void Jump()
    {
        if (_isGrounded && Input.GetKeyDown(KeyBinds.Jump) && _timer >= _jumpInputTime)
        {
            _rb.AddForce(Mathf.Sqrt(jumpHeight * _rb.mass * _rb.drag * -20 * _gravity) * Vector3.up, ForceMode.Impulse);
            _timer = 0;
        }
        else if(_timer < _jumpInputTime)
        {
            _timer += Time.deltaTime;
        }
    }

    private void DoGroundCheck()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groudMask);
    }
}
