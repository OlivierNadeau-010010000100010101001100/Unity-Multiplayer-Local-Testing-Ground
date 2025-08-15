using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;

    private void Awake()
    {
        serverBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
            Debug.Log("Server Started");
        });

        hostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            Debug.Log("Host Started");
        });

        clientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            Debug.Log("Client Started");
        });
    }
}
