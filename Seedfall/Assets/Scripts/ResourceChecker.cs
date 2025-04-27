using UnityEngine;

public class ResourceChecker : MonoBehaviour
{
    void Start()
    {
        // Check if resources exist
        GameObject player1 = Resources.Load<GameObject>("Player1Square");
        GameObject player2 = Resources.Load<GameObject>("Player2Square");

        Debug.Log("Player1Square resource found: " + (player1 != null));
        Debug.Log("Player2Square resource found: " + (player2 != null));

        // List all resources in the Resources folder
        Object[] allResources = Resources.LoadAll("");
        Debug.Log("Found " + allResources.Length + " resources:");
        foreach (Object obj in allResources)
        {
            Debug.Log("- " + obj.name + " (" + obj.GetType() + ")");
        }
    }
}