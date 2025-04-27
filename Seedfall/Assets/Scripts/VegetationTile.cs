using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VegetationTile : MonoBehaviour
{
    public static int highTiles;
    public static int deforestedTiles;

    [SerializeField] private List<VegetationState> possibleStates = new List<VegetationState>();
    [SerializeField] private VegetationState currentState;

    private bool isProducingMoney;
    void Start()
    {
        ChangeState("Low");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeState(string stateName)
    {
        if (currentState != null && currentState.stateName == stateName)
            return;

        currentState = possibleStates[FindStateIndex(stateName)];
        Sprite randomSprite = currentState.stateSprites[Random.Range(0, currentState.stateSprites.Count)];
        GetComponent<SpriteRenderer>().sprite = randomSprite;

        if (currentState.stateName == "Deforested")
        {
            isProducingMoney = false;
            highTiles = Mathf.Max(0, highTiles - 1);
            deforestedTiles++;
            GameManager.ManagerInstance.tilesTextPanel.transform.Find("DeforestedText").GetComponent<TextMeshProUGUI>().text = "Deforested Tiles: " + deforestedTiles;
            GameManager.ManagerInstance.tilesTextPanel.transform.Find("HighVegetationText").GetComponent<TextMeshProUGUI>().text = "High Vegetation Tiles: " + highTiles;

            // Lose condition when 80% of area is deforested
            if (GameManager.ManagerInstance.deforestedTilesToLose == deforestedTiles)
            {
                GameManager.ManagerInstance.GameFinished("Lose");
            }

            StopCoroutine(AffectMoney());
        }

        else if(currentState.stateName == "High")
        {
            isProducingMoney = true;
            highTiles++;
            GameManager.ManagerInstance.tilesTextPanel.transform.Find("HighVegetationText").GetComponent<TextMeshProUGUI>().text = "High Vegetation Tiles: " + highTiles;

            if (GameManager.ManagerInstance.highTilesToWin == highTiles)
            {
                GameManager.ManagerInstance.GameFinished("Win");
            }

            StartCoroutine(AffectMoney());
        }
        else
        {
            isProducingMoney = true;
            StartCoroutine(AffectMoney());
        }
    }

    public bool UpgradeTile() 
    {
        int currentStateIndex = FindStateIndex(currentState.stateName);
        if (currentState.stateName != "High") 
        {
            if (GameManager.ManagerInstance.GetComponent<Money>().PayAmount(currentState.moneyCostToUpgrade) == false) 
            {
                Debug.Log("Not enough money to upgrade this tile");
                return false;
            }
            ChangeState(possibleStates[currentStateIndex + 1].stateName);
            return true;
        } 
        else
        {
            Debug.Log("Can't upgrade this tile anymore");
            return false;
        }
    }

    private int FindStateIndex(string stateName) 
    {
        for (int i = 0; i < possibleStates.Count; i++)
        {
            if (possibleStates[i].stateName == stateName)
            {
                return i;
            }
        }

        Debug.Log("Could not find vegetation state");
        return 0; // just return anything if no state found
    }

    IEnumerator AffectMoney() 
    {
        while(isProducingMoney) {
            yield return new WaitForSeconds(currentState.affectMoneyInterval);
            GameManager.ManagerInstance.gameObject.GetComponent<Money>().AddAmount(currentState.moneyAmountEffect);
        }
    }
}
