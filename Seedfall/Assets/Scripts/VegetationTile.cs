using UnityEngine;

public class VegetationTile : MonoBehaviour
{
    public enum VegetationLevel { Deforested, Low, Medium, High};

    [SerializeField] private Sprite lowLevelSprite;
    [SerializeField] private VegetationLevel level;

    void Start()
    {
        level = VegetationLevel.Deforested;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Deforest()
    {
        level = VegetationLevel.Deforested;
        GetComponent<SpriteRenderer>().sprite = lowLevelSprite;
    }
}
