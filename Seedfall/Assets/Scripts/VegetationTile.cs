using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetationTile : MonoBehaviour
{
    [SerializeField] private List<VegetationState> possibleStates = new List<VegetationState>();
    [SerializeField] private VegetationState currentState;

    private bool isProducingMoney;
    void Start()
    {
        ChangeState("Deforested");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeState(string stateName)
    {
        currentState = possibleStates[FindStateIndex(stateName)];
        GetComponent<SpriteRenderer>().sprite = currentState.stateSprite;

        if (currentState.stateName == "Deforested")
        {
            isProducingMoney = false;
            StopCoroutine(AffectMoney());
        }
        else
        {
            isProducingMoney = true;
            StartCoroutine(AffectMoney());
        }
    }

    public void UpgradeTile() 
    {
        int currentStateIndex = FindStateIndex(currentState.stateName);

        if (currentState.stateName != "High") 
        {
            ChangeState(possibleStates[currentStateIndex + 1].stateName);
        } 
        else
        {
            Debug.Log("Can't upgrade this tile anymore");
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
