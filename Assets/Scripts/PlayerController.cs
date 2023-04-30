using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector2 moveVector;
    public float speed = 2f;
    public float runSpeed = 5f;
    
    public Animator anim;
    public float jumpForce = 7f;
    public Transform groundCheck;
    public float checkRadius = 0.5f;
    public LayerMask ground;
	public bool isGrounded;
    public int maxJumpValue = 2; //Максимум прыжков
    public float laungeImpulse = 5000f; //Сила рывка
    private bool blockMoveX = false;
	private bool facingRight = true;
    //private bool jumpControl; //Проверка нахождения в контролируемом прыжке
    //private int jumpIteration = 0; //Считает сколько мы были в прыжке
    //private int jumpValueIteration = 60; //Максимум сколько может длиться усиление прыжка
    private int jumpCount = 0; //Счётчик сколько сделано прыжков
	private float realSpeed;
	// Start is called before the first frame update
	void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
    }

	private void Start()
	{
        realSpeed = speed;
	}

	// Update is called once per frame
	void Update()
    {
        Walk();
        Run(); 
        Jump();
        CheckingGround();
        Lunge();
        SquatCheck();
	}

    void Walk()
    {
        if (!blockMoveX)
        {
            moveVector.x = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(moveVector.x * realSpeed, rb.velocity.y);
            anim.SetFloat("moveX", Mathf.Abs(moveVector.x));
            //rb.AddForce(moveVector * speed);
            if (moveVector.x > 0 && !facingRight)
            {
                Flip();
            }
            else if (moveVector.x < 0 && facingRight)
            {
                Flip();
            }
        }
    }
    private bool speedLock;
    void Run()
    {
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            anim.SetBool("run", true);
            realSpeed = runSpeed;
            if (Input.GetKeyDown(KeyCode.Space)) { speedLock = true; }
        }
        else
        {
			anim.SetBool("run", false);
			if (speedLock) { realSpeed = speed; }
            else if (speedLock && isGrounded) { speedLock = false; }
            else { realSpeed = runSpeed; }
		}
    }

    void Jump()
    {
		//      if (Input.GetKey(KeyCode.Space))
		//      {
		//	if (isGrounded)
		//	{
		//		jumpControl = true;
		//	}
		//}
		//      else
		//      {
		//          jumpControl= false;

		//}

		//      if (jumpControl)
		//      {
		//          if (jumpIteration++ < jumpValueIteration)
		//          {
		//              rb.AddForce(Vector2.up * jumpForce / jumpIteration);
		//          }
		//      }
		//      else
		//      {
		//          jumpIteration=0;
		//      }
        if (!jumpLock)
        {
			if (isGrounded || jumpCount < maxJumpValue) //Двойной прыжок
			{
				if (Input.GetKeyDown(KeyCode.Space))
				{

					if (!isGrounded)
					{
						anim.StopPlayback();
						anim.Play("DoubleJump");
					}
					else
					{
						anim.StopPlayback();
						anim.Play("Jump");
					}


					jumpCount++;

					rb.velocity = new Vector2(rb.velocity.x, 0);
					rb.AddForce(Vector2.up * jumpForce);


				}
			}
			if (isGrounded)
			{
				jumpCount = 1;
			}
		}
		


        if (Input.GetKeyDown(KeyCode.S)) //Спуск вниз с платформы
        {
            Physics2D.IgnoreLayerCollision(7, 8, true);
            Invoke("IgnoreLayerOff", 0.5f); //Возвращает возможность встать на платформу
        }

	}

    void IgnoreLayerOff()
    {
		Physics2D.IgnoreLayerCollision(7, 8, false);
	}


	void Flip()
    {
        facingRight = !facingRight;
        Vector2 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void CheckingGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, ground);
		anim.SetBool("isJumping", !isGrounded);
	}

	void Lunge()
	{
		if (Input.GetKeyDown(KeyCode.LeftAlt) && !lockLounge)
		{
            lockLounge = true;
            rb.velocity = new Vector2(0, 0);
            if (rb.transform.localScale.x < 0) { rb.AddForce(Vector2.left * laungeImpulse); }
            else { rb.AddForce(Vector2.right * laungeImpulse); }
            Invoke("LoungeLock", 2f);
		}

	}

    private bool lockLounge = false;

	void LoungeLock()
	{
        lockLounge = false;
	}

    public Transform topCheck;
    private float topCheckRadius = 0.5f;
    public LayerMask roof;
    //public Collider2D poseStand;
    //public Collider2D poseSquat;
    private bool jumpLock = false;

    private void SquatCheck()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            anim.SetBool("squat", true);
			jumpLock = true;
		}
        else if (!Physics2D.OverlapCircle(topCheck.position, topCheckRadius, roof))
        {
			anim.SetBool("squat", false);
			jumpLock = false;
		}
    }

  
}

