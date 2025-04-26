using UnityEngine;

public class TempPlayer : MonoBehaviour
{

    private Camera cam;
    
    void Start()
    {
        cam = FindAnyObjectByType<Camera>();
    }

    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), cam.ScreenToWorldPoint(Input.mousePosition));
            
            if(hit.collider.CompareTag("VegetationTile"))
            {
                hit.collider.GetComponent<VegetationTile>().Deforest();
            }
        }
    }
}
