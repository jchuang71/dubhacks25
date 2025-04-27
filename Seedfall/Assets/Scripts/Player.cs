using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon.StructWrapping; // Import the Photon Unity Networking namespace

public class Player : MonoBehaviourPun
{
    float speed = 5f; // Speed of the player movement
    public VegetationArea vegetationArea; // Reference to the VegetationArea script
    public GameObject tileStandingOn; // Reference to the tile the player is standing on
    public int currentSprite = 0; // Index of the current sprite
    private bool isWalking = false; // Flag to check if the player is walking
    private float timeToCycleSprite = 0.1f; // Time interval to change the sprite
    private float timeSinceLastSpriteChange = 0f; // Time since the last sprite change
    private int playerNumber;
    public Sprite[] idleSprites = new Sprite[7]; // Array to hold the idle sprites
    public Sprite[] walkSprites = new Sprite[6]; // Array to hold the walking sprites


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        idleSprites = Resources.LoadAll<Sprite>("Sprites/Player Sprites/Player" + playerNumber + "IdleSprites"); // Folder path under Resources
        walkSprites = Resources.LoadAll<Sprite>("Sprites/Player Sprites/Player" + playerNumber + "WalkSprites"); // Folder path under Resources
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
        }
        else if(Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector2(-1,0) * Time.deltaTime * speed); // Move left
        }
        else if(Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector2(0,-1) * Time.deltaTime * speed); // Move down
        }
        else if(Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector2(1,0) * Time.deltaTime * speed); // Move right
        }
    }

    void PlayerMendForest()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(tileStandingOn != null)
            {
                tileStandingOn.GetComponent<VegetationTile>().UpgradeTile(); // Change the state of the tile to "Low"
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Player collided with: " + collision.gameObject.name); // Log the name of the collided object
        tileStandingOn = collision.gameObject;
    }
}
