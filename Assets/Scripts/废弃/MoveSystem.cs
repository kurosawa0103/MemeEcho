using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSystem : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anima;
    public float speed;
    public float jumpForce;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public bool canMove;
    public bool isGrounded;
    private float stopSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anima = transform.GetChild(0).transform.GetComponent<Animator>();
    }
    private void FixedUpdate()
    {

        if(canMove)
        {
            stopSpeed = 1;
            GroundMovement();
        }
        if (!canMove)
        {
            stopSpeed = 0;
            GroundMovement();
            //anima.SetFloat("moveSpeed", 0);
        }
        // 检测玩家是否在地面上
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 2f, groundLayer);

        // 处理跳跃
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    //玩家移动方法
    void GroundMovement()
    {
        //获取按钮输入
        float horizontalMove = Input.GetAxisRaw("Horizontal"); 
        //执行物理移动
        rb.velocity = new Vector2(horizontalMove * speed*stopSpeed, rb.velocity.y);
        //如果按下任意移动按钮
        if (horizontalMove != 0)
        {
            //如果可以移动则切换朝向
            if(canMove)
            {
                transform.localScale = new Vector3(horizontalMove * 1.0f, 1.0f, 1.0f);
               
            }
            //anima.SetFloat("moveSpeed", 1);
           
        }
        //如果没按移动按钮，则关闭脚步声，移动动画切换为idle
        if (horizontalMove == 0)
        {
            //anima.SetFloat("moveSpeed", 0);
        }
    }

    
   
}
