using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Variables
    public float runSpeed = 5f;
    public float jumpForce = 5f;

    public Rigidbody2D body;
    public Animator animator;

    public bool isGrounded;
    public Transform groundCheckPoint;
    public float groundCheckRadius;
    public LayerMask groundLayer;

    // Movement
    public bool jumpPressed = false;
    public bool crouchPressed = false;
    public bool canStand = false;
    public bool APressed = false;
    public bool DPressed = false;
    public float currentSpeed = 0f;

    [Header("Colliders")]
    public BoxCollider2D defaultCollider;
    public BoxCollider2D crouchCollider;
    [Header("Ceiling Check")]
    public float ceilingCheckHeight = 0.2f; // Высота проверки потолка
    public LayerMask obstacleLayer; // Слой препятствий над головой

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        CheckCeiling();
        CheckInputs();
        SetAnimation();
    }
    void CheckInputs()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) jumpPressed = true;
        if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.LeftControl))
        {
            crouchPressed = true;
        }
        else
        {
            if (canStand)
            {
                crouchPressed = false;
            }
        }

            APressed = Input.GetKey(KeyCode.A);
        DPressed = Input.GetKey(KeyCode.D);
    }
    void SetAnimation()
    {
        currentSpeed = body.linearVelocity.x;

        animator.SetFloat("Speed", Mathf.Abs(currentSpeed));
        animator.SetBool("IsJumping", !isGrounded);
        animator.SetBool("IsCrouching", crouchPressed);
        animator.SetBool("CanStandUp", canStand);
    }
    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        float targetSpeed = 0f;

        if (APressed)
        {
            targetSpeed = -runSpeed;
        }
        else if (DPressed)
        {
            targetSpeed = runSpeed;
        }

        if (crouchPressed)
        {
            defaultCollider.enabled = false;
            crouchCollider.enabled = true;
            targetSpeed = targetSpeed * 0.4f;
        }
        else if (canStand)
        {
            defaultCollider.enabled = true;
            crouchCollider.enabled = false;
        }

        if (!crouchPressed && !canStand)
        {
            crouchPressed = true;
        }


        body.linearVelocity = new Vector2(targetSpeed, body.linearVelocity.y);

        // Поворот персонажа
        if (APressed) transform.eulerAngles = new Vector3(0, 180, 0);
        else if (DPressed) transform.eulerAngles = new Vector3(0, 0, 0);
        // Прыжок
        if (jumpPressed && !crouchPressed)
        {
            body.linearVelocity = new Vector2(body.position.x, jumpForce);
            jumpPressed = false;
        }
    }
    void CheckCeiling()
    {
        if (!crouchPressed)
        {
            // Получаем границы коллайдера в мировых координатах
            Bounds colliderBounds = defaultCollider.bounds;
            // Рассчитываем стартовую позицию проверки
            Vector2 checkStart = colliderBounds.center + new Vector3(0, colliderBounds.extents.y, 0);
            // Рассчитываем конечную позицию проверки
            Vector2 checkEnd = checkStart + new Vector2(0, ceilingCheckHeight);

            // Проверяем область и рисуем линию
            canStand = !Physics2D.OverlapArea(checkStart, checkEnd, obstacleLayer);
        }
        else
        {
            canStand = true;
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
    }
}