using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField]
    private Button startHostBtn;

    [SerializeField]
    private Button startServerBtn;

    [SerializeField]
    private Button startClientBtn;

    private void Awake()
    {
        startHostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });

        startServerBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        });

        startClientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }
}
