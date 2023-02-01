using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator animController;
    float horizontal_value;
    float vertical_value;
    Vector2 ref_velocity = Vector2.zero;
    float jumpForce = 12f;

    [SerializeField] float moveSpeed_horizontal = 400.0f;
    [SerializeField] bool is_jumping = false;
    [SerializeField] bool can_jump = false;
    [Range(0, 1)] [SerializeField] float smooth_time = 0.5f;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private bool hasVerticalChanged;
    [SerializeField] private bool hasHorizontalChanged;
    private bool canDash = true;
    private bool isDashing;
    private float horizontaldashingPower = 25f;
    private float verticaldashingPower = 25f;
    private float dashingTime = 0.5f;
    private float dashingCooldown = 0.3f;
    private bool wallJumped = false;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animController = GetComponent<Animator>();
        //Debug.Log(Mathf.Lerp(current, target, 0));
    }

    // Update is called once per frame
    void Update()
    {

        if (isDashing)
        {
            return;
        }
        horizontal_value = Input.GetAxis("Horizontal");

        if (horizontal_value > 0) sr.flipX = false;
        else if (horizontal_value < 0) sr.flipX = true;

        if (isDashing)
        {
            return;
        }
        /*vertical_value = Input.GetAxis("Verticale");

                if (vertical_value > 0) sr.flipY = false;
                else if (vertical_value < 0) sr.flipY = true;*/


        animController.SetFloat("Speed", Mathf.Abs(horizontal_value));

        if (Input.GetButtonDown("Jump") && can_jump)
        {
            is_jumping = true;
            animController.SetBool("Jumping", true);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }
    void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        if (is_jumping && can_jump)
        {
            is_jumping = false;
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            can_jump = false;
        }
        Vector2 target_velocity = new Vector2(horizontal_value * moveSpeed_horizontal * Time.fixedDeltaTime, rb.velocity.y);
        rb.velocity = Vector2.SmoothDamp(rb.velocity, target_velocity, ref ref_velocity, 0.05f);

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        can_jump = true;
        animController.SetBool("Jumping", false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        animController.SetBool("Jumping", false);
    }


    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        //rb.gravityScale = 0f;

        if (Input.GetKey(KeyCode.Q))
        {
            rb.velocity = new Vector2(transform.localScale.x * -horizontaldashingPower, 0f);
        }

        else if (Input.GetKey(KeyCode.D))
        {
            rb.velocity = new Vector2(transform.localScale.x * horizontaldashingPower, 0f);
        }



        if (Input.GetKey(KeyCode.Z))
        {
            rb.velocity = new Vector2(0f, transform.localScale.y * horizontaldashingPower);
            
        }

        else if (Input.GetKey(KeyCode.S))
        {
            rb.velocity = new Vector2(0f, transform.localScale.y * -horizontaldashingPower);
            
        }

        Debug.Log("sale noir");



        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true; 
    }
}





//if (hasVerticalChanged)