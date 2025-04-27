using Unity.VisualScripting;
using Photon.Pun;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager ManagerInstance; // Singleton reference to this manager object available to all other scripts
    public VegetationArea vegetationArea;
    public UpgradeMenuBehavior upgradeMenuBehavior;

    public float pollutionInterval = 2.0f;
    //public GameObject playerPrefab; // Reference to the player prefab

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.Instantiate("Prefabs/Player", new Vector3(0, 0, -1), Quaternion.identity, 0); // Instantiate the player prefab at the origin with no rotation

        if (ManagerInstance == null)
            ManagerInstance = this;

        StartCoroutine(Pollution());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            upgradeMenuBehavior.GetComponent<UpgradeMenuBehavior>().ToggleMenu();
        }
    }

    IEnumerator Pollution() {
        while(true)
        {
            yield return new WaitForSeconds(pollutionInterval);
            int randomIndex = Random.Range(0, vegetationArea.tiles.Count - 1);
            vegetationArea.tiles[randomIndex].GetComponent<VegetationTile>().ChangeState("Deforested");
        }
    }
}
