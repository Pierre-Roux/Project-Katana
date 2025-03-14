using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour
{
    [Header("Carac")]
    public float speed;
    public float MaxHealth;
    public float JumpForce;
    public int nbDoubleJump;

    [Header("Params")]
    public CinemachineVirtualCamera VirtualCam;
    public GameObject groundCheckPosition;
    public LayerMask whatIsLayerGround;

    private float Health;
    private float isDead;
    private bool isFacingLeft;
    private Rigidbody2D rb;
    private float DirectionX;
    private bool isGrounded;
    private int doubleJumpCounter;

    // Start is called before the first frame update
    void Start()
    {
        Health = MaxHealth;
        rb = GetComponent<Rigidbody2D>();
        SetFacing(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Facing Charactere
        DirectionX = Input.GetAxisRaw("Horizontal");
        if (DirectionX == -1f && isFacingLeft == false)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, 180f, transform.rotation.z);
            SetFacing(true);
        }
        else if (DirectionX == 1f && isFacingLeft == true)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, 0f, transform.rotation.z);
            SetFacing(false);
        }

        isGrounded = Physics2D.OverlapCircle(groundCheckPosition.transform.position, 0.3f, whatIsLayerGround);

        Debug.Log(isGrounded);

        if (Input.GetKeyDown("space"))
        {
            if (isGrounded)
            {
                rb.velocity = Vector2.up * JumpForce;
            }
            else if (doubleJumpCounter > 0)
            {
                rb.velocity = Vector2.up * JumpForce;
                doubleJumpCounter--;
            }
        }

        if (isGrounded)
        {
            doubleJumpCounter = nbDoubleJump;
        }
    }

    void FixedUpdate()
    {
        rb.position += new Vector2(DirectionX * speed * Time.deltaTime, rb.velocity.y);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheckPosition.transform.position, 0.3f);
    }

    public void SetFacing(bool FacingLeft)
    {
        isFacingLeft = FacingLeft;
    }
}
