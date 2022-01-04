using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask platformLayerMask;
    //[SerializeField] private int cherries = 0;
    [SerializeField] private Text cherryText;

    public int cherries;
    float horizontalMove=0f;
    public float Speed = 30f;
    public float jumpForce = 16f;
    public bool facingRight = true;
    public Animator animator;
    private Rigidbody2D m_Rigidbody2D;
    private CircleCollider2D isGround;
    bool jump = false;
    private bool jumped=false;
    private bool hurt;
    public float hurtForce = 0.1f;
    private bool m_Grounded;            // Whether or not the player is grounded.

    public UnityEvent OnLandEvent;

    // Start is called before the first frame update
    void Start()
    {

        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        isGround = GetComponent<CircleCollider2D>();
        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }

    // Update is called once per frame
    void Update()
    {
        //checks whether user pressed left arrow key or right arrow key
        horizontalMove=Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        //if user pressed spacebar start jump animation
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            animator.SetBool("Jumping", true);
        }
        if (m_Rigidbody2D.transform.position.y < -6f)
        {
            Debug.Log("Game over");
        }
    }

    void OnCollisionEnter(Collision theCollision)
    {
        if (theCollision.gameObject.name == "Foreground")
        {
            m_Grounded = true;
        }
    }

    void OnCollisionExit(Collision theCollision)
    {
        if (theCollision.gameObject.name == "Foreground")
        {
            m_Grounded = false;
        }
    }


    void FixedUpdate()
    {

        
        CheckIfGrounded();

        //RaycastHit2D slopeHit = CheckIfSlope();
        RaycastHit2D slopeHit;
        //cast ray to check if checracter is climbing on slope
        Vector3 position = isGround.bounds.center;
        position.y = position.y - 0.5f;
        //check which direction character is facing and based on direction draw ray to chech wether it is on a slope
        if (facingRight)
        {
            slopeHit = Physics2D.Raycast(position, Vector2.right, 0.8f, platformLayerMask);
            //Debug.DrawRay(position, Vector2.right, Color.red, 0.8f);
        }
        else
        {
            slopeHit = Physics2D.Raycast(position, Vector2.left, 0.8f, platformLayerMask);
            //Debug.DrawRay(position, Vector2.left, Color.red, 0.8f);
        }

        float slopeAngle = Vector2.Angle(slopeHit.normal, Vector2.up);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad);
        //if it is on a slope and spacebar is pressed enable jump
        if (slopeHit.collider && slopeAngle<80 && jump)
        {
            m_Rigidbody2D.gravityScale = 3.5f;
            m_Rigidbody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            jump = false;
            jumped = true;
        }
        //if it is on a slope move manually
        else if (slopeHit.collider && slopeAngle < 80)
        {
            if (jumped)
            {
                jumped = false;
                OnLanding();
            }
            m_Rigidbody2D.gravityScale = 0.0f;
            Debug.Log(m_Rigidbody2D.gravityScale);
            Debug.Log(horizontalMove);
            Debug.Log(climbVelocityY);
            transform.position += new Vector3(horizontalMove, climbVelocityY, 0) * Time.fixedDeltaTime * Speed;
        }
        //if it is not on a slope turn gravity back on and move it
        else
        {
            if (m_Rigidbody2D.gravityScale==0.0f)
            {
                m_Rigidbody2D.gravityScale = 3.5f;
            }
            if ((Mathf.Abs(m_Rigidbody2D.velocity.x)<0.1f))
            {
                //calculating movement to left or right
                transform.position += new Vector3(horizontalMove, 0, 0) * Time.fixedDeltaTime * Speed;
            }

        }



        //turn character to face other side if needed
        if (horizontalMove > 0 && !facingRight)
        {
            Flip();
        }
        else if(horizontalMove<0 && facingRight)
        {
            Flip();
        }

        if (jump && m_Grounded)
        {
            //add force so character jumps
            m_Rigidbody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            jump = false;
        }
    }

    private void CheckIfGrounded()
    {
        //store if player was grounded in previous frame
        bool wasGrounded = m_Grounded;
        m_Grounded = false;
        float extraHeight = 1f;
        //casting circleRay to check if character is touching anything on Foreground layer
        RaycastHit2D raycastHit = Physics2D.CircleCast(isGround.bounds.center, isGround.radius, Vector2.down, extraHeight, platformLayerMask);
        if (raycastHit.collider != null)
        {
            m_Grounded = true;
            //if charecter just tuched the ground trigger OnLandEvent
            if (!wasGrounded)
                OnLandEvent.Invoke();
        }
    }

    private RaycastHit2D CheckIfSlope()
    {
        RaycastHit2D hit;
        //cast ray to check if checracter is climbing on slope
        Vector3 position = isGround.bounds.center;
        position.y = position.y - 0.5f;
        //check which direction character is facing and based on direction draw ray to chech wether it is on a slope
        if (facingRight)
        {
            hit = Physics2D.Raycast(position, Vector2.right, 0.8f, platformLayerMask);
            Debug.DrawRay(position, Vector2.right, Color.red, 0.8f);
        }
        else
        {
            hit = Physics2D.Raycast(position, Vector2.left, 0.8f, platformLayerMask);
            Debug.DrawRay(position, Vector2.left, Color.red, 0.8f);
        }
        return hit;
    }

    //function for collecting cherry objects
    private void OnTriggerEnter2D(Collider2D theCollision)
    {
        if(theCollision.tag == "Collectible")
        {
            Destroy(theCollision.gameObject);
            cherries += 1;
            cherryText.text = cherries.ToString();
        }
    }

    //Collision with enemy
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            if(m_Grounded == false)
            {
                Destroy(other.gameObject);
                m_Rigidbody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            }
            // player is hurt
            else
            {
                hurt = true;
                Animator a = other.gameObject.GetComponent<Animator>();
                a.SetTrigger("Crash");
                if (other.gameObject.transform.position.x > transform.position.x)
                {
                    m_Rigidbody2D.velocity = new Vector2(-hurtForce, 0);
                }
                else
                {
                    m_Rigidbody2D.velocity = new Vector2(hurtForce, 0);
                }
            }
        }
    }

    //function for turning character to face other side if needed
    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    //called when OnLandEvent is triggered
    public void OnLanding()
    {
        animator.SetBool("Jumping", false);
    }
}
