using System.Collections.Generic;
using UnityEngine;

public class VegetationTile : MonoBehaviour
{
    [SerializeField] private List<VegetationState> possibleStates = new List<VegetationState>();
    [SerializeField] private VegetationState currentState;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeState(string stateName)
    {
        currentState = FindState(stateName);
        GetComponent<SpriteRenderer>().sprite = currentState.stateSprite;
    }

    private VegetationState FindState(string stateName) 
    {
        foreach (VegetationState state in possibleStates) 
        {
            if (state.stateName == stateName)
            {
                return state;
            }
        }

        Debug.Log("Could not find vegetation state");
        return possibleStates[0]; // just return anything if no state found
    }
}
