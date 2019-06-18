using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonStage1 : MonoBehaviour
{
    [SerializeField] GameObject enemyProjectile1;
    [SerializeField] AudioClip fireBreath;
    [SerializeField] float projectileSpeed = 5;
    [SerializeField] float minTimeBetweenShots = 1f;
    [SerializeField] float maxTimeBetweenShots = 3f;

    Animator animator;
    float shotCounter;

    // Start is called before the first frame update
    void Start()
    {
        shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndShoot();

    }

    //  TIMER FOR DRAGON TO SHOOT.
    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
      
        if (shotCounter <= 0f)
        {
            Fire();
            animator.SetTrigger("IsAttacking");
            AudioSource.PlayClipAtPoint(fireBreath, Camera.main.transform.position);

            shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        }
    }


    private void Fire()
    {
            GameObject fireBall = Instantiate(enemyProjectile1, new Vector3(80f, -2f, 0), Quaternion.identity) as GameObject;
            fireBall.GetComponent<Rigidbody2D>().velocity = new Vector2(-projectileSpeed, 0);
    }

}
