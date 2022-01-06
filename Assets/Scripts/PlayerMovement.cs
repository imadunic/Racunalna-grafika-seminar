using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask platformLayerMask;
    //[SerializeField] private int cherries = 0;
    [SerializeField] private Text cherryText;
    [SerializeField] private Text lifesText;

    public int cherries;
    float horizontalMove = 0f;
    public float Speed = 30f;
    public float jumpForce = 16f;
    public bool facingRight = true;
    public Animator animator;
    private Rigidbody2D m_Rigidbody2D;
    private CircleCollider2D isGround;
    private int lifes;
    bool jump = false;
    public float hurtForce = 10f;
    private bool m_Grounded;            // Whether or not the player is grounded.

    public UnityEvent OnLandEvent;

    // Start is called before the first frame update
    void Start()
    {

        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        lifes = 4;
        isGround = GetComponent<CircleCollider2D>();
        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }

    // Update is called once per frame
    void Update()
    {
        if ((m_Rigidbody2D.transform.position.y < -6f) || (lifes<=0) )
        {
            horizontalMove = 0;
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
            jump = false;
            SceneManager.LoadScene("GameOver");
            //code for end of game
        }
        else
        {
            //checks whether user pressed left arrow key or right arrow key
            horizontalMove = Input.GetAxisRaw("Horizontal");
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
            //if user pressed spacebar start jump animation
            if (Input.GetButtonDown("Jump"))
            {
                jump = true;
                animator.SetBool("Jumping", true);
            }
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
        Vector3 position = isGround.bounds.center;
        position.y = position.y - 0.7f;
        RaycastHit2D hitFront;
        RaycastHit2D hitBack;
        if (facingRight)
        {
            hitFront = Physics2D.Raycast(position, Vector2.right, 1f, platformLayerMask);
            hitBack = Physics2D.Raycast(position, Vector2.left, 1f, platformLayerMask);
        }
        else
        {
            hitFront = Physics2D.Raycast(position, Vector2.left, 1f, platformLayerMask);
            hitBack = Physics2D.Raycast(position, Vector2.right, 1f, platformLayerMask);
        }
        float slopeAngle = Vector2.Angle(hitFront.normal, Vector2.up);
        if (hitFront.collider && slopeAngle < 90)
        {
            m_Rigidbody2D.gravityScale = 0.0f;

            float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad);
            transform.position += new Vector3(horizontalMove, climbVelocityY, 0) * Time.fixedDeltaTime * Speed;
        }
        else
        {
            m_Rigidbody2D.gravityScale = 3.5f;

            if (Mathf.Abs(m_Rigidbody2D.velocity.x) < 0.01f)
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
        else if (horizontalMove < 0 && facingRight)
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
        float extraHeight = 1.5f;
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


    //function for collecting cherry objects
    private void OnTriggerEnter2D(Collider2D theCollision)
    {
        if (theCollision.tag == "Collectible")
        {
            Destroy(theCollision.gameObject);
            cherries += 1;
            cherryText.text = cherries.ToString();
        }
    }

    //Collision with enemy
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (m_Grounded == false)
            {
                Destroy(other.gameObject);
                m_Rigidbody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            }
            // player is hurt
            else
            {
                lifes -= 1;
                lifesText.text = lifes.ToString();
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