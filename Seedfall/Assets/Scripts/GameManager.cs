using Unity.VisualScripting;
using Photon.Pun;
using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager ManagerInstance; // Singleton reference to this manager object available to all other scripts
    public VegetationArea vegetationArea;
    public UpgradeMenuBehavior upgradeMenuBehavior;
    public GameObject gameEndPanel;
    public GameObject tilesTextPanel;
    public TextMeshProUGUI goalText;

    public int highTilesToWin;
    public int deforestedTilesToLose;

    public float playerMoveSpeed = 5.0f;
    public float pollutionInterval = 5.0f;

    private bool pollutionEnabled;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.Instantiate("Prefabs/Player", new Vector3(0, 0, -1), Quaternion.identity, 0); // Instantiate the player prefab at the origin with no rotation

        highTilesToWin = Mathf.RoundToInt(vegetationArea.tiles.Count * 0.6f);
        deforestedTilesToLose = Mathf.RoundToInt(vegetationArea.tiles.Count * 0.8f);

        goalText.text = "Goal: " + highTilesToWin;

        gameEndPanel.SetActive(false);
        pollutionEnabled = true;

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
        while(pollutionEnabled)
        {
            yield return new WaitForSeconds(pollutionInterval);
            int randomIndex = Random.Range(0, vegetationArea.tiles.Count);
            vegetationArea.tiles[randomIndex].GetComponent<VegetationTile>().ChangeState("Deforested");
        }
    }

    public void GameFinished(string winOrLose)
    {
        pollutionEnabled = false;
        GetComponent<EventManager>().eventsEnabled = false;

        gameEndPanel.transform.Find("WinLoseText").GetComponent<TextMeshProUGUI>().text = "You " + winOrLose + "!";

        if (winOrLose == "Lose")
        {
            gameEndPanel.transform.Find("DetailsText").GetComponent<TextMeshProUGUI>().text = "Deforested Tiles: " + VegetationTile.deforestedTiles + " / " + vegetationArea.tiles.Count;
        }
        else
        {
            gameEndPanel.transform.Find("DetailsText").GetComponent<TextMeshProUGUI>().text = "High Vegetation Tiles: " + VegetationTile.highTiles + " / " + vegetationArea.tiles.Count;
        }

        gameEndPanel.SetActive(true);
    }
}
