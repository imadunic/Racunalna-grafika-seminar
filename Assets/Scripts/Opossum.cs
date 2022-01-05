using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opossum : MonoBehaviour
{
    private bool facingLeft = true;
    private Animator anim;
    [SerializeField] private float speed = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("Left", true);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (facingLeft)
        {
            transform.position += new Vector3(-1, 0,0) * Time.fixedDeltaTime * speed; ;
        }
        else
        {
            transform.position += new Vector3(1, 0, 0) * Time.fixedDeltaTime * speed;
        }
    }

    private void TurnLeft()
    {
        if (!facingLeft)
        {
            Flip();
            anim.SetBool("Left", true);
        }
    }

    private void TurnRight()
    {
        Debug.Log("Right");
        if (facingLeft)
        {
            Flip();
            anim.SetBool("Left", false);
        }
    }

    private void Flip()
    {
        facingLeft = !facingLeft;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
