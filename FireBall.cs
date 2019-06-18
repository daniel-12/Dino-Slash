using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    Rigidbody2D myRigidBody;
    Knight knight;
    Dragon dragon;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        knight = FindObjectOfType<Knight>();
        dragon = FindObjectOfType<Dragon>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (myRigidBody.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        {
            Destroy(gameObject);
        }
        if (myRigidBody.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            Destroy(gameObject);
        }
    }

    bool IsFacingRight()
    {
        return transform.localScale.x > 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var wall = myRigidBody.IsTouchingLayers(LayerMask.GetMask("Floor"));
        
        // IF FIRE PROJECTILE HITS THE WALL, BOUNCE OFF.
        if (wall)
        {
            transform.localScale = new Vector2(-(Mathf.Sign(myRigidBody.velocity.x)), 1f);

            if (IsFacingRight())
            {
                myRigidBody.velocity = new Vector2(10, 0f);
            }
            else
            {
                myRigidBody.velocity = new Vector2(-10, 0f);
            }
        }
        
        
    }
}
