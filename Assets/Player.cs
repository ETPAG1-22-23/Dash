using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float dashSpeed = 5.0f;
    Rigidbody2D rb;
    private Vector2 dashDirection;
    SpriteRenderer sr;
    Animator animController;

    float horizontal_value;
    float vertical_value;
    float current = 50.0f;
    float target = 300.0f;
    Vector2 ref_velocity = Vector2.zero;

    [SerializeField] float moveSpeed_horizontal = 400f;
    [SerializeField] bool is_jumping = false;
    [SerializeField] bool can_jump = false;
    float jumpForce = 10f;
    [Range(0, 1)] [SerializeField] float smooth_time = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animController = GetComponent<Animator>();
        Debug.Log(Mathf.Lerp(current, target, 0));
    }

    // Update is called once per frame
    void Update()
    {
        horizontal_value = Input.GetAxis("Horizontal");

        animController.SetFloat("speed", Mathf.Abs(horizontal_value));
        sr.flipX = horizontal_value < 0;
        animController.SetFloat("fall", rb.velocity.y);

        vertical_value = Input.GetAxis("Vertical");
        if (Input.GetButtonDown("Jump") && can_jump)
        {
            is_jumping = true;
            animController.SetBool("jumping", true);
        }

        // Récupération de la position de la souris
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Calcul de la direction de dash en utilisant la position de la souris
        dashDirection = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);
        dashDirection.Normalize();

        if (Input.GetMouseButtonDown(0))
        {
            
            Dash();
            
        }
    }
    void FixedUpdate()
    {
        if (is_jumping && can_jump)
        {
            is_jumping = false;
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            can_jump = false;
        }
        Vector2 target_velocity = new Vector2(horizontal_value * moveSpeed_horizontal * Time.fixedDeltaTime, rb.velocity.y);
        rb.velocity = Vector2.SmoothDamp(rb.velocity, target_velocity, ref ref_velocity, 0.05f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        can_jump = true;
        Debug.Log(collision.gameObject.tag);
        animController.SetBool("jumping", false);

       
        
    }
    private void Dash()
    {


        rb.AddForce(dashDirection * dashSpeed, ForceMode2D.Impulse);

        

        // Déplacement du personnage en utilisant la méthode AddForce
        
    }

}