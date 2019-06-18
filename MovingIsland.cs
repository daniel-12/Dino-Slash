using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingIsland : MonoBehaviour
{
    Rigidbody2D myRigidBody;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myRigidBody.velocity = new Vector2(-2, 0f);    
    }

    // Update is called once per frame
    void Update()
    {

    }

    bool IsGoingRight()
    {
        return myRigidBody.velocity.x > 0;
    }

    // MAKE THE MOVING ISLAND BOUNCE OFF THE WALL AND HEAD THE OPPOSITE DIRECTION.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var wall = collision.gameObject.tag == "wall";


        if (wall)
        {

            if (IsGoingRight())
            {
                myRigidBody.velocity = new Vector2(-2, 0f);
            }
            else
            {
                myRigidBody.velocity = new Vector2(2, 0f);
            }
        }

    }
}
