using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField]
    private float moveSpeed = 3f;

    void Update()
    {
        if (!this.IsOwner)
        {
            return;
        }

        Vector3 moveDirection = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W)) moveDirection = Vector3.forward;
        if (Input.GetKey(KeyCode.S)) moveDirection = Vector3.back;
        if (Input.GetKey(KeyCode.A)) moveDirection = Vector3.left;
        if (Input.GetKey(KeyCode.D)) moveDirection = Vector3.right;
        this.transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
}
