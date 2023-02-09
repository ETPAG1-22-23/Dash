using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator animController;
    Vector2 ref_velocity = Vector2.zero;
    float moveSpeedHorizontal = 400f;
    float horizontalValue;
    float verticalValue;


    [SerializeField] int dashLimit = 1;
    [SerializeField] int currentDash;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] bool isJumping = false;
    [SerializeField] bool canJump = false;
    [SerializeField] private bool canDash = true;
    [SerializeField] private bool isDashing;
    [SerializeField] private bool hasVerticalChanged;
    [SerializeField] private float horizontalDashingPower = 24f;
    [SerializeField] private float verticalDashingPower = 14f;
    [SerializeField] private float dashingTime = 0.001f;
    [SerializeField] private float dashingCooldown = 1f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animController = GetComponent<Animator>();
        currentDash = dashLimit;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing)
        {
            return;
        }

        // handle sprite horizontal orientation
        horizontalValue = Input.GetAxis("Horizontal");
        animController.SetBool("Running", horizontalValue!=0);
        animController.SetFloat("speed", Mathf.Abs(horizontalValue));
        
        if (horizontalValue > 0) sr.flipX = false;
        else if (horizontalValue < 0) sr.flipX = true;
        
        verticalValue = Input.GetAxis("Vertical");
        
        hasVerticalChanged = verticalValue != 0;
        if (verticalValue > 0) sr.flipY = false;
        else if (verticalValue < 0) sr.flipY = true;

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            isJumping = true;
            animController.SetBool("Jumping", true);
        }

        animController.SetFloat("Speed", Mathf.Abs(horizontalValue));

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && currentDash > 0)
        {
            StartCoroutine(Dash());
        }

        Debug.Log(rb.velocity);
    }
    void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        if (isJumping)
        {
            isJumping = false;
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            canJump = false;
        }
        Vector2 target_velocity = new Vector2(horizontalValue * moveSpeedHorizontal * Time.fixedDeltaTime, rb.velocity.y);
        rb.velocity = Vector2.SmoothDamp(rb.velocity, target_velocity, ref ref_velocity, 0.05f);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        currentDash = dashLimit;
        canJump = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        animController.SetBool("Jumping", false);
    }
    private IEnumerator Dash()
    {
        /* currentDash = currentDash - 1;
        currentDash -= 1; */
        currentDash--;
        canDash = false;
        // facing left 
        if (sr.flipX == true)
        {
            rb.velocity += new Vector2(transform.localScale.x * -horizontalDashingPower, transform.localScale.y);
        }
        else
        {
            rb.velocity += new Vector2(transform.localScale.x * horizontalDashingPower, transform.localScale.y);
        }
        
        if (hasVerticalChanged)
        {
            //facing down
            if (sr.flipY == true)
            {
                rb.velocity += new Vector2(transform.localScale.x, transform.localScale.y * -verticalDashingPower);
            }
            else
            {
                rb.velocity += new Vector2(transform.localScale.x, transform.localScale.y * verticalDashingPower);
            }
        }

        yield return new WaitForSeconds(dashingTime);
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}