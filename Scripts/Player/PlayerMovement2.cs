using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    public float XYSpeed;
    public float XYSpeedLimit;

    public Boundary boundary;

    private Rigidbody2D rb;
    private Vector3 movement;
    private float h;
    private float v;

    private Animator anim;

    // Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Use for physics
    void FixedUpdate()
    {
        if (!PlayerController2.Player2Dead)
        {
            h = Input.GetAxisRaw("Horizontal2");
            v = Input.GetAxisRaw("Vertical2");

            if (Mathf.Abs(h) > 0.0f || Mathf.Abs(v) > 0.0f)
            {
                if (anim.speed < 1.3f)
                {
                    anim.speed += 0.005f;
                }
            }
            else if (anim.speed - 0.01f >= 0.0f)
            {
                anim.speed -= 0.01f;
            }

            else
            {
                anim.speed = 0.0f;
            }

            // Add force if under speed limit
            if (h > 0 && rb.velocity.x < XYSpeedLimit || h < 0 && rb.velocity.x > -XYSpeedLimit)
            {
                movement.Set(h * XYSpeed, 0, 0);
                rb.AddForce(movement, ForceMode2D.Impulse);
            }
            if (v > 0 && rb.velocity.y < XYSpeedLimit || v < 0 && rb.velocity.y > -XYSpeedLimit)
            {
                movement.Set(0, v * XYSpeed, 0);
                rb.AddForce(movement, ForceMode2D.Impulse);
            }

            // Keep speed within limits
            // x
            if (rb.velocity.x > XYSpeedLimit)
            {
                rb.velocity = new Vector2(XYSpeedLimit, rb.velocity.y);
            }
            else if (rb.velocity.x < -XYSpeedLimit)
            {
                rb.velocity = new Vector2(-XYSpeedLimit, rb.velocity.y);
            }

            // y
            if (rb.velocity.y > XYSpeedLimit)
            {
                rb.velocity = new Vector2(rb.velocity.x, XYSpeedLimit);
            }
            else if (rb.velocity.y < -XYSpeedLimit)
            {
                rb.velocity = new Vector2(rb.velocity.x, -XYSpeedLimit);
            }

            rb.position = new Vector2
            (
                Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
                Mathf.Clamp(rb.position.y, boundary.yMin, boundary.yMax)
            );
        }
        else
        {
            anim.speed = 0f;
        }
    }
}
