using UnityEngine;

public class SubirEscada : MonoBehaviour
{
    [Header("Movimento")]
    public float climbSpeed = 4f;

    private Rigidbody2D rb;
    private float verticalInput;
    private bool canClimb;
    private bool isClimbing;
    private float gravityBeforeClimb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gravityBeforeClimb = rb.gravityScale;
    }

    void Update()
    {
        verticalInput = Input.GetAxisRaw("Vertical");

        if (canClimb && Mathf.Abs(verticalInput) > 0.1f)
        {
            isClimbing = true;
        }

        if (!canClimb)
        {
            isClimbing = false;
        }
    }

    void FixedUpdate()
    {
        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(rb.velocity.x, verticalInput * climbSpeed);
        }
        else
        {
            rb.gravityScale = gravityBeforeClimb;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Escadas"))
        {
            canClimb = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Escadas"))
        {
            canClimb = false;
            isClimbing = false;
        }
    }
}