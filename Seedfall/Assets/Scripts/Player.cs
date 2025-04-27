using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon.StructWrapping;
using NUnit.Framework; // Import the Photon Unity Networking namespace

public class Player : MonoBehaviourPun
{
    //float speed = 5f; // Speed of the player movement
    // public VegetationArea vegetationArea; // Reference to the VegetationArea script
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
            if(timeSinceLastSpriteChange >= timeToCycleSprite) // Check if the time since the last sprite change is greater than or equal to the time to cycle the sprite
            {
                timeSinceLastSpriteChange = 0f; // Reset the time since last sprite change
                CycleSprites(); // Call the CycleSprites function every frame
            }
            timeSinceLastSpriteChange += Time.deltaTime; // Increment the time since last sprite change by the time since the last frame
        }
    }

    void PlayerMovement()
    {
        isWalking=false; // Reset the walking flag to false
        float speed = GameManager.ManagerInstance.playerMoveSpeed; // Get the player move speed from the GameManager

        if(Input.GetKey(KeyCode.W))
        {
            transform.Translate(new Vector2(0,1) * Time.deltaTime * speed); // Move up
            SetSpriteToWalk(); // Set the sprite to walking animation
        }
        if(Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector2(-1,0) * Time.deltaTime * speed); // Move left
            SetSpriteToWalk(); // Set the sprite to walking animation
            gameObject.GetComponent<SpriteRenderer>().flipX = true; // Flip the sprite to face left
        }
        if(Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector2(0,-1) * Time.deltaTime * speed); // Move down
            SetSpriteToWalk(); // Set the sprite to walking animation
        }
        if(Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector2(1,0) * Time.deltaTime * speed); // Move right
            SetSpriteToWalk(); // Set the sprite to walking animation
            gameObject.GetComponent<SpriteRenderer>().flipX = false; // Flip the sprite to face right
        }
    }

    void PlayerMendForest()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            tileStandingOn = GameManager.ManagerInstance.vegetationArea.GetTileAtPosition(transform.position);
            if(tileStandingOn != null)
            {
                bool success = tileStandingOn.GetComponent<VegetationTile>().UpgradeTile();
            }
        }
    }

    void SetSpriteToWalk()
    {
        isWalking = true; // Set the walking flag to true
        //gameObject.GetComponent<SpriteRenderer>().sprite = walkSprites[0]; // Set the sprite to the first walking sprite
        //currentSprite = 0; // Reset the current sprite index to 0
    }

    void SetSpriteToIdle()
    {
        isWalking = false; // Set the walking flag to false
        //gameObject.GetComponent<SpriteRenderer>().sprite = idleSprites[0]; // Set the sprite to the first idle sprite
        //currentSprite = 0; // Reset the current sprite index to 0
    }

    void CycleSprites()
    {
        if (isWalking) // Check if the player is walking
        {
            // Move to the next sprite
            currentSprite = (currentSprite + 1) % walkSprites.Length;
            gameObject.GetComponent<SpriteRenderer>().sprite = walkSprites[currentSprite]; // Set the sprite to the current walking sprite
        }
        else
        {
            // Move to the next sprite
            currentSprite = (currentSprite + 1) % idleSprites.Length;
            gameObject.GetComponent<SpriteRenderer>().sprite = idleSprites[currentSprite]; // Set the sprite to the current idle sprite
        }
    }
}
