using UnityEngine;

public class Player : MonoBehaviour
{
    float speed = 5f; // Speed of the player movement
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement(); // Call the PlayerMovement function every frame
    }

    void PlayerMovement()
    {
        if(Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed); // Move forward
        }
        else if(Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * Time.deltaTime * speed); // Move left
        }
        else if(Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * Time.deltaTime * speed); // Move backward
        }
        else if(Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * Time.deltaTime * speed); // Move right
        }
    }
}
