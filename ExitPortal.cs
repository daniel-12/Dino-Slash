using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortal : MonoBehaviour
{
    [SerializeField] AudioClip exitSound;

    /*  LOAD NEXT SCENE WITH A 1 SECOND DELAY, TIME SLOWED AND A SOUND EFFECT. */

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine("Delay");
    }

    IEnumerator Delay()
    {
        AudioSource.PlayClipAtPoint(exitSound, Camera.main.transform.position);
        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(1);
        Time.timeScale = 1;

        var currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene + 1);
    }
}
