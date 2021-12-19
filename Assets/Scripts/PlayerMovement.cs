using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    //public CharacterController2D controller;
    float horizontalMove=0f;
    public float Speed = 30f;
    bool facingRight = true;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //checks whether user pressed left arrow key or right arrow key
        horizontalMove=Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
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
    }



    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
