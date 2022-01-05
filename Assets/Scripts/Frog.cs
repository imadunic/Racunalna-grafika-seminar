using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : MonoBehaviour
{

    private Animator anim;
    private bool m_Grounded=true;
    private BoxCollider2D isGround;
    [SerializeField] private LayerMask platformLayerMask;
    [SerializeField] private float JumpHeight=5f;
    [SerializeField] private float JumpLength=2f;
    private int counter;
    private Rigidbody2D rb;
    private bool facingLeft = true;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        isGround = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        counter = 0;

    }

    // Update is called once per frame
    void Update()
    {
        CheckIfGrounded();
    }

    private void Jump()
    {
        anim.SetBool("Jumping", true);
        if (facingLeft)
        {
            rb.velocity = new Vector2(-JumpLength, JumpHeight);
        }
        else
        {
            rb.velocity = new Vector2(JumpLength, JumpHeight);
        }
        counter++;

    }

    private void CheckIfGrounded()
    {
        //store if player was grounded in previous frame
        bool wasGrounded = m_Grounded;
        m_Grounded = false;
        float extraHeight = 1f;
        //casting circleRay to check if character is touching anything on Foreground layer
        RaycastHit2D raycastHit = Physics2D.BoxCast(isGround.bounds.center, isGround.size,0f, Vector2.down, extraHeight, platformLayerMask);
        if (raycastHit.collider != null)
        {
            m_Grounded = true;
            //if charecter just tuched the ground trigger OnLandEvent
            if (!wasGrounded)
            {
                anim.SetBool("Jumping", false);
                if (counter == 2)
                {
                    Flip();
                    counter = 0;
                }
            }
        }
    }

    private void Flip()
    {
        facingLeft= !facingLeft;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
