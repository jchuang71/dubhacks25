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

    public void Deforest()
    {
        GetComponent<SpriteRenderer>().sprite = currentState.stateSprite;
    }
}
