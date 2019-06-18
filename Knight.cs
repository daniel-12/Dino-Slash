using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;

public class Knight : MonoBehaviour
{
    float timer;
    bool isAlive = true;
    public Slider healthBar;
    public GameObject winCanvas;
    Animator animator;
    BoxCollider2D myFeet;
    Dragon dragon;
    Rigidbody2D myRigidBody;

    [SerializeField] int runspeed = 5;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] int health = 10;
    [SerializeField] int damage = 1;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip wackSound;
    [SerializeField] AudioClip victorySound;


    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        myFeet = GetComponent<BoxCollider2D>();
        dragon = FindObjectOfType<Dragon>();

        healthBar.value = health;
        winCanvas.SetActive(false);
        timer = 0f;
    }

    
    // Update is called once per frame
    void Update()
    {
        if (dragon != null)
        {
            if (!isAlive) { return; } 

            Run();
            Jump();
            Attack();

            var touchingEnemy = myFeet.IsTouchingLayers(LayerMask.GetMask("Enemy"));

            if (!touchingEnemy) { return; }

            /* IF MY FEET ARE TOUCHING THE ENEMY'S HEAD, STUN HIM. */
            else
            {
                StartCoroutine("DontTouchIt");
            }

            if (health <= 0)
            {
                Destroy(gameObject);
                healthBar.value = health;
                Death();
            }
        }
        // IF THE ENEMY == NULL, CELEBRATE.
        else
        {
            GetComponent<Animator>().SetBool("HasWon", true);
            winCanvas.SetActive(true);
            timer -= Time.deltaTime;

            // PLAY CELEBRATORY AUDIO ONCE EVERY 12 SECONDS USING TIMER.
            if (timer <= 0)
            {
                AudioSource.PlayClipAtPoint(victorySound, Camera.main.transform.position);
                timer = 12f;
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        var isFireBall = collision.GetComponent<FireBall>();
        var touchingEnemy = myFeet.IsTouchingLayers(LayerMask.GetMask("Enemy"));

        // IF WE TOUCH THE FIRE PROJECTILE TAKE DAMAGE.
        if (isFireBall)
        {
            health -= damage;
            healthBar.value = health;
            StartCoroutine("Danger");

            if (health <= 0)
            {
                Destroy(gameObject);
                Death();
            }
        }

        if (touchingEnemy)
        {
            AudioSource.PlayClipAtPoint(wackSound, Camera.main.transform.position);
        }
    }


    /*  SLIDE OFF THE ENEMY'S HEAD TO PREVENT CHEATING. ALSO, FLIP THE SPRITE SO THAT ENEMY IS ALWAYS FACING THE PLAYER.*/

    IEnumerator DontTouchIt()
    {
        yield return new WaitForSeconds(0.1f);
        var touchingEnemy = myFeet.IsTouchingLayers(LayerMask.GetMask("Enemy"));
        
        if (touchingEnemy)
        {
            if (transform.localScale.x == -1)
            {
                transform.Translate(Vector2.left * 5 * Time.deltaTime);
            }
            else
            {
                transform.Translate(Vector2.right * 5 * Time.deltaTime);
            }
        }
    }



    IEnumerator Danger()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }

 

    private void Attack()
    {
        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("IsAttacking");
            AudioSource.PlayClipAtPoint(wackSound, Camera.main.transform.position);
        }
    }

    // ONLY JUMP WHEN TOUCHING THE FLOOR OR THE ENEMY'S HEAD.
    private void Jump()
    {
        var touchingFloor = myFeet.IsTouchingLayers(LayerMask.GetMask("Floor"));
        var touchingEnemy = myFeet.IsTouchingLayers(LayerMask.GetMask("Enemy"));

        if (touchingFloor)
        {
            if (CrossPlatformInputManager.GetButtonDown("Jump"))
            {
                Vector2 jump = new Vector2(0f, jumpSpeed);
                myRigidBody.velocity += jump;
                animator.SetTrigger("IsJumping");
                AudioSource.PlayClipAtPoint(jumpSound, Camera.main.transform.position);
            }
        }
    }

    private void Run()
    {
        float horizontalVar = CrossPlatformInputManager.GetAxis("Horizontal");
        Vector2 playerVelocity = new Vector2(horizontalVar * runspeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;

        // HAS VELOCITY BOOL
        bool playerHasVelocity = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;

        // IF PLAYER HAS VELOCITY ADD RUNNING ANIMATION. FLIP THE SPRITE ACCORDINGLY.
        if (playerHasVelocity)
        {
            animator.SetBool("IsRunning", playerHasVelocity);
            transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
    }

    public bool Death()
    {
        return isAlive = false;
    }
}
