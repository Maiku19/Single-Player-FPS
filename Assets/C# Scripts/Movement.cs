using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject body;

    public float acc = 1;
    public float topSpeed = 10;

    public float jumpHeight = 2f;

    bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groudMask;

    public float moveX = 0f;
    public float moveY = 0f;

    Rigidbody rb;
    float timer = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
    }

    private void Update()
    {
        #region GroundCheck
        //Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groudMask);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space) && timer >= 0.1f)
        {
            rb.velocity = transform.up * jumpHeight;
            timer = 0;
        }
        #endregion
    }

    private void FixedUpdate()
    {
        

        Vector3 force = body.transform.right * moveX;
        force += body.transform.forward * moveY;

        rb.AddForce(force);

        timer += Time.fixedDeltaTime;
    }

    void OnMikeSphereTriggerEnter(RaycastHit hit)
    {
        //transform.position = hit.normal + hit.point * GetComponent<CapsuleCollider>().radius;
    }
}
