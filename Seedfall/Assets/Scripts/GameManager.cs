using Unity.VisualScripting;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager ManagerInstance; // Singleton reference to this manager object available to all other scripts
    public VegetationArea vegetationArea;
    //public GameObject playerPrefab; // Reference to the player prefab

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.Instantiate("Prefabs/Player", new Vector3(0, 0, -1), Quaternion.identity, 0); // Instantiate the player prefab at the origin with no rotation
        ManagerInstance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
