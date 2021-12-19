using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    //public CharacterController2D controller;
    float horizontalMove=0f;
    public float Speed = 30f;
    public float jumpForce = 0.3f;
    bool facingRight = true;
    public Animator animator;
    private Rigidbody2D m_Rigidbody2D;
    bool jump = false;
    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //checks whether user pressed left arrow key or right arrow key
        horizontalMove=Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            //animator.SetBool("IsJumping", true);
        }
    }

    void FixedUpdate()
    {
        transform.position += new Vector3(horizontalMove, 0, 0) * Time.fixedDeltaTime * Speed;
        if (horizontalMove > 0 && !facingRight)
        {
            Flip();
        }
        else if(horizontalMove<0 && facingRight)
        {
            Flip();
        }

        if (jump)
        {
            m_Rigidbody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            jump = false;
        }
    }



    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
