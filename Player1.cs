using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player1 : MonoBehaviour
{
    [SerializeField] int runspeed = 5;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip wackSound;
    Rigidbody2D myRigidBody;
    BoxCollider2D myFeet;
    Animator animator;
    MovingIsland island;
    [SerializeField] AudioClip gameOverSound;


    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        myFeet = GetComponent<BoxCollider2D>();
        island = FindObjectOfType<MovingIsland>();
    }

    // Update is called once per frame
    void Update()
    {
        Run();
        Jump();
        Attack();
        if (!island) { return; }
        var touchingIsland = myRigidBody.IsTouchingLayers(LayerMask.GetMask("Island"));
        var islandSpeed = island.GetComponent<Rigidbody2D>().velocity;

        if (touchingIsland)
        {
            myRigidBody.velocity += islandSpeed;
        }
    }

    private void Attack()
    {
        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("IsAttacking");
            AudioSource.PlayClipAtPoint(wackSound, Camera.main.transform.position);
        }
    }

    private void Jump()
    {
        // ONLY JUMP WHEN TOUCHING FLOOR.
        var touchingFloor = myFeet.IsTouchingLayers(LayerMask.GetMask("Floor"));
        var touchingIsland = myFeet.IsTouchingLayers(LayerMask.GetMask("Island"));
        var someElse = myFeet.IsTouchingLayers(LayerMask.GetMask("SomeElse"));

        if (touchingFloor || touchingIsland || someElse)
        {
            // CROSS PLATFORM INPUT MANAGER SO IT CAN BE PLAYED ON DIFFERENT CONSOLES.
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
        bool playerHasVelocity = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;

        // IF THE PLAYER HAS VELOCITY PLAY RUNNING ANIMATION AND FLIP THE SPRITE  LEFT OR RIGHT ACCORDINGLY.
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

    private void OnTriggerEnter2D(Collider2D collision)
    {

       var gotHit = myRigidBody.IsTouchingLayers(LayerMask.GetMask("Projectile"));
        var hazard = collision.gameObject.tag == "Hazards";

        if (hazard || gotHit)
        {
            StartCoroutine("Delay");
            GetComponent<SpriteRenderer>().color = Color.clear;
        }
    }

    IEnumerator Delay()
    {
        AudioSource.PlayClipAtPoint(gameOverSound, Camera.main.transform.position);
        Time.timeScale = 0.2f;
        yield return new WaitForSeconds(1);
        Time.timeScale = 1;
        var currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
        Destroy(gameObject);
    }
}
