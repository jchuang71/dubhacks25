using UnityEngine;

public class VegetationTile : MonoBehaviour
{
    public enum VegetationLevel { Deforested, Low, Medium, High};

    public VegetationLevel level;
    void Start()
    {
        level = VegetationLevel.Low;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
