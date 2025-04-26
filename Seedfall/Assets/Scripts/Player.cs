using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon.StructWrapping; // Import the Photon Unity Networking namespace

public class Player : MonoBehaviourPun
{
    float speed = 5f; // Speed of the player movement
    public VegetationArea vegetationArea; // Reference to the VegetationArea script
    public GameObject tileStandingOn; // Reference to the tile the player is standing on
    private int playerNumber;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerNumber = PhotonNetwork.LocalPlayer.ActorNumber;
    }

    // Update is called once per frame
    void Update()
    {
        //tileStandingOn = null;
        if(photonView.IsMine) // Check if this player instance is controlled by the local player
        {
            PlayerMovement(); // Call the PlayerMovement function every frame
            PlayerMendForest(); // Call the PlayerMendForest function every frame
        }
    }

    void PlayerMovement()
    {
        if(Input.GetKey(KeyCode.W))
        {
            transform.Translate(new Vector2(0,1) * Time.deltaTime * speed); // Move up
            SetSpriteToWalk(); // Set the sprite to walking animation
        }
        else if(Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector2(-1,0) * Time.deltaTime * speed); // Move left
            SetSpriteToWalk(); // Set the sprite to walking animation
            if(gameObject.GetComponent<SpriteRenderer>().flipX == false) // Check if the sprite is not flipped
                gameObject.GetComponent<SpriteRenderer>().flipX = true; // Flip the sprite to face left
        }
        else if(Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector2(0,-1) * Time.deltaTime * speed); // Move down
            SetSpriteToWalk(); // Set the sprite to walking animation
        }
        else if(Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector2(1,0) * Time.deltaTime * speed); // Move right
            SetSpriteToWalk(); // Set the sprite to walking animation
            if(gameObject.GetComponent<SpriteRenderer>().flipX == true) // Check if the sprite is flipped
                gameObject.GetComponent<SpriteRenderer>().flipX = false; // Flip the sprite to face right
        }
        else
        {
            SetSpriteToIdle(); // Set the sprite to idle animation
        }
    }

    void PlayerMendForest()
    {
        if(Input.GetKey(KeyCode.E))
        {
            if(tileStandingOn != null)
            {
                tileStandingOn.GetComponent<VegetationTile>().ChangeState("High"); // Change the state of the tile to "Low"
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Player collided with: " + collision.gameObject.name); // Log the name of the collided object
        tileStandingOn = collision.gameObject;
    }

    void SetSpriteToWalk()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Player Sprites/Player " + playerNumber + " walk");
    }

    void SetSpriteToIdle()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Player Sprites/Player " + playerNumber + " idle");
    }
}
