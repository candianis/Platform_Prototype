using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float forcejump;
    //[SerializeField] float buttonTime;
    //[SerializeField] float jumpTime;
    public bool jumping;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb=GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        JumpPlayer();
    }
    void MovePlayer()
    {
        float speedInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(speedInput * speed, rb.velocity.y);
    }
    void JumpPlayer()
    {  
        if (Input.GetButton("Jump")&& jumping==false)
        {
            rb.AddForce(new Vector2(rb.velocity.x, forcejump));
            //jumping = true;
            //jumpTime = 0;
        }
        //if (jumping)
        //{
        //    rb.AddForce(new Vector2(rb.velocity.x, forcejump));
        //    jumpTime += 2/*Time.deltaTime*/;
        //}
        //if (jumpTime > buttonTime)
        //{
        //    jumping = false;
        //}
    }
  
}
