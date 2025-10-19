using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Danger"))
        {
            SceneManager.LoadScene(0);
        }
        if (collision.gameObject.CompareTag("Finish"))
        {
            SceneManager.LoadScene(0);
            // Planning to lead next level in the future
        }


    }
}
