using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;

public class Dragon : MonoBehaviour
{
    [SerializeField] GameObject enemyProjectile1;
    [SerializeField] GameObject enemyProjectile2;
    [SerializeField] AudioClip fireBreath;
    [SerializeField] float projectileSpeed = 5;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [SerializeField] int health = 200;
    [SerializeField] int damage = 1;

    CapsuleCollider2D myHead;
    Rigidbody2D myRigidBody;
    Animator animator;
    Knight knight;

    public Slider healthBar;
    bool isAlive = true;
    public GameObject winCanvas;
    float shotCounter;


    private void Start()
    {
        // SHOT COUNTER SET TO RANDOM. FOR ENEMY PROJECTILES.
        shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        myHead = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        knight = FindObjectOfType<Knight>();
        healthBar.value = health;
        winCanvas.SetActive(false);
        myRigidBody = GetComponent<Rigidbody2D>();
    }



    private void Update()
    {
        // IF KNIGHT IS DEAD, DRAGON CELEBRATES.
        if (knight == null)
        {
            GetComponent<Animator>().SetBool("HasWon", true);
            winCanvas.SetActive(true);
            return;
        }

        if (!isAlive) { return; }

        // IF PLAYER JUMPS OVER ME, FLIP MY SPRITE.
        if (myHead.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            transform.localScale = new Vector2(-(transform.localScale.x), 1f);
        }

        CountDownAndShoot();
        IsAttacking();

        healthBar.value = health;

        FireDamage();
    }                         

    /* ENEMY WILL SHOOT RANDOMLY USING THE SHOT COUNTER. IF ENEMY IS HIT IN THE HEAD AND STUNNED THE SHOT COUNTER WILL BE SET TO 5 TO PREVENT
       SHOOTING FOR THE NEXT 5 SECONDS. TIME.DELTATIME IS SUBTRACTED TO BRING DOWN THE COUNTER AND MAKE FRAME RATE INDEPENDENT. */
    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime; 
        var fireball = myRigidBody.IsTouchingLayers(LayerMask.GetMask("Projectile"));   
        if (fireball) { shotCounter = 5; }
        if (shotCounter <= 0f)
        {
                Fire();
            animator.SetTrigger("IsAttacking");
            AudioSource.PlayClipAtPoint(fireBreath, Camera.main.transform.position);

            shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        }
    }

    // IF WE'RE FACING LEFT FIRE TO THE LEFT AND FIRE TO THE RIGHT WHEN WE'RE FACING RIGHT.
    private void Fire()
    {
       if (transform.localScale.x == -1)
       {
            GameObject fireBall = Instantiate(enemyProjectile1, new Vector3(-1.5f,-3.5f,0), Quaternion.identity) as GameObject;
            fireBall.GetComponent<Rigidbody2D>().velocity = new Vector2(-projectileSpeed, 0);
       }
       else
       {
            GameObject fireBall = Instantiate(enemyProjectile2, new Vector3(1.5f, -3.5f, 0), Quaternion.identity) as GameObject;
            fireBall.GetComponent<Rigidbody2D>().velocity = new Vector2(projectileSpeed, 0);
        }
    }

    // LET THE KNIGHT INFLICT DAMAGE WHEN WE'RE AT STRIKING DISTANCE.
    public void IsAttacking()
    {
        var knightPosition = knight.transform.position.x;

        if (knightPosition > -2.5 && knightPosition < 2.5)
        {
            if (CrossPlatformInputManager.GetButtonDown("Fire1"))
            {
                health -= damage;
                GetComponent<SpriteRenderer>().color = Color.red;

                if (health <= 0)
                {
                    Destroy(gameObject);
                    Death();
                }
            }
            else
            {
                GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }

    public void victory()
    {
            knight.GetComponent<Animator>().SetBool("HasWon", true);
    }

    public bool Death()
    {
        return isAlive = false;
    }

    private void FireDamage()
    {
        var fireball = myRigidBody.IsTouchingLayers(LayerMask.GetMask("Projectile"));   

        if (fireball)  
        {
            StartCoroutine("Stunned");     

            health -= damage;          
            healthBar.value = health;

            if (health <= 0)
            {
                Destroy(gameObject);
                Death();
            }
        }
    }

    // PLAY STUN ANIMATION FOR 1 SECONDS.
    IEnumerator Stunned()
    {
        GetComponent<Animator>().SetBool("IsStunned", true);
        yield return new WaitForSeconds(5);
        GetComponent<Animator>().SetBool("IsStunned", false);
    }

}
