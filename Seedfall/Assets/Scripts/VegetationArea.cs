using UnityEngine;

public class VegetationArea : MonoBehaviour
{
    [SerializeField] private GameObject tile;
    [SerializeField] private int areaWidth = 5;
    [SerializeField] private int areaHeight = 5;
    [SerializeField] private int cellSize = 1;

    private Camera cam;
    void Start()
    {
        cam = FindAnyObjectByType<Camera>();
        transform.localScale = new Vector2(areaWidth, areaHeight);
    }

    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if(IsPositionWithinBounds(ray.origin))
                Instantiate(tile, SnapPositionToGrid(ray.origin), Quaternion.identity);
        }

    }

    Vector2 SnapPositionToGrid(Vector2 pos)
    {
        Vector2 newPos = new Vector2(Mathf.Round((pos.x / cellSize) * cellSize), Mathf.Round((pos.y / cellSize) * cellSize));
        return newPos;
    }

    bool IsPositionWithinBounds(Vector2 pos)
    {
        float xMin = transform.position.x - (areaWidth / 2.0f);
        float xMax = transform.position.x + (areaWidth / 2.0f);
        float yMin = transform.position.y - (areaHeight / 2.0f);
        float yMax = transform.position.y + (areaHeight / 2.0f);

        Debug.Log("Xmin: " + xMin);
        Debug.Log("Xmax: " + xMax);
        Debug.Log("Ymin: " + yMin);
        Debug.Log("Ymax: " + yMax);

        if (pos.x >= xMin && pos.x <= xMax && pos.y >= yMin && pos.y <= yMax)
            return true;
        return false;
    }
}
