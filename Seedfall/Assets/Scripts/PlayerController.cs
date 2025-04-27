using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun
{
    public float moveSpeed = 5f;

    void Update()
    {
        if (photonView == null)
        {
            Debug.LogError("PhotonView is null on PlayerController!");
            return;
        }

        // Only control this player if it's ours
        if (photonView.IsMine)
        {
            // Get input
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // Calculate movement
            Vector2 movement = new Vector2(horizontal, vertical) * moveSpeed * Time.deltaTime;

            // Apply movement
            transform.Translate(movement);

            // Debug logging for movement
            if (horizontal != 0 || vertical != 0)
            {
                Debug.Log($"Moving player: h={horizontal}, v={vertical}");
            }
        }
    }
}