using UnityEngine;
using System.Collections.Generic;

public class VegetationArea : MonoBehaviour
{
    public List<GameObject> tiles;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int areaWidth = 5;
    [SerializeField] private int areaHeight = 5;
    [SerializeField] private int cellSize = 1;

    private Camera cam;

    void Start()
    {
        cam = FindAnyObjectByType<Camera>();

        // Create vegetation area by specified width and height
        for(int x = 0; x < areaWidth; x++)
        {
            for(int y = 0; y < areaHeight; y++)
            {
                Vector2 newPos = new Vector2(x + GetXMin() + 0.5f, y + GetYMin() + 0.5f);
                GameObject newTile = Instantiate(tilePrefab, newPos, Quaternion.identity, transform);
                tiles.Add(newTile);
            }
        }
    }

    public GameObject GetTileAtPosition(Vector2 pos)
    {
        Vector2 objectPos = SnapPositionToGrid(pos);
        foreach(GameObject tile in tiles)
        {
            if(tile.transform.position.x == objectPos.x && tile.transform.position.y == objectPos.y)
            {
                return tile;
            }
        }

        return null;
    }

    public Vector2 SnapPositionToGrid(Vector2 pos)
    {
        return new Vector2(Mathf.Round((pos.x / cellSize) * cellSize), Mathf.Round((pos.y / cellSize) * cellSize));
    }

    bool IsPositionWithinBounds(Vector2 pos)
    {
        return pos.x >= GetXMin() && pos.x <= GetXMax() && pos.y >= GetYMin() && pos.y <= GetYMax();
    }

    float GetXMin()
    {
        return transform.position.x - (areaWidth / 2.0f);
    }

    float GetXMax()
    {
        return transform.position.x + (areaWidth / 2.0f);
    }

    float GetYMin()
    {
        return transform.position.y - (areaHeight / 2.0f);
    }

    float GetYMax()
    {
        return transform.position.y + (areaHeight / 2.0f);
    }

    // Editor area size visual helper
    private void OnDrawGizmos()
    {
        Gizmos.DrawLineList(new Vector3[4]
        {
            new Vector2(GetXMin(), GetYMin()),
            new Vector2(GetXMax(), GetYMax()),
            new Vector2(GetXMin(), GetYMax()),
            new Vector2(GetXMax(), GetYMin())
        });
    }
}
