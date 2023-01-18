using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] Transform _movementDirectionReference;
    public float speed = 1;
    public float topSpeed = 10;
    public float jumpHeight = 2f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groudMask;
    [SerializeField, InspectorName("Jump Input Time Tolerance")] float _jumpInputTime = 0.25f;

    bool isGrounded;
    Rigidbody rb;
    float timer = 0;
    float velocityY = 0;

    const float gravity = -9.81f;
    
    private void Start()
    {
        if(!TryGetComponent(out rb)) { Debug.LogError($"{this} requires {typeof(Rigidbody)} to function"); };
        rb.velocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        // TODO: Add movement deceleration

        DoGroundCheck();
        SimulateGravity();
        Move();
    }

    private void Update()
    {
        Jump();
    }

    private void Move()
    {
        // Input
        Vector3 force = KeyBinds.Movement.x * _movementDirectionReference.right + KeyBinds.Movement.y * _movementDirectionReference.forward;
        rb.velocity = Vector3.ClampMagnitude(force * speed, topSpeed);

        rb.velocity += velocityY * Vector3.up;
    }

    private void SimulateGravity()
    {
        velocityY += gravity * Time.deltaTime;
        if (isGrounded && velocityY < 0) { velocityY = 0; }
    }

    private void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyBinds.Jump) && timer >= _jumpInputTime)
        {
            velocityY = Mathf.Sqrt(jumpHeight * -2 * gravity);
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    private void DoGroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groudMask);
    }
}
