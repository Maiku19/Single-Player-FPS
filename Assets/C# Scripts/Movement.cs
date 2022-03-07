using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject body;

    public float speed = 1;
    public float topSpeed = 10;

    public float jumpHeight = 2f;

    bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groudMask;
    [SerializeField] private float gravity = -9.81f;

    Rigidbody rb;
    float timer = 0;
    float velocityY = 0;

    private void Start()
    {
        if(!TryGetComponent(out rb)) { Debug.LogError($"{this} requires {typeof(Rigidbody)} to function"); };
        rb.velocity = Vector3.zero;
    }

    private void Update()
    {

        #region jump
        //Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groudMask);

        if (isGrounded && Input.GetKeyDown(KeyBinds.Jump) && timer >= 0.1f)
        {
            velocityY = Mathf.Sqrt(jumpHeight * -2 * gravity);
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
        }
        #endregion

        velocityY += gravity * Time.deltaTime;
        if(isGrounded && velocityY < 0) { velocityY = 0; }


        Vector3 force = KeyBinds.Movement.x * transform.right + KeyBinds.Movement.y * transform.forward;
        rb.AddForce(force * speed * rb.drag * rb.mass);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, topSpeed);

        rb.velocity += velocityY * Vector3.up * Time.deltaTime;
    }
}
