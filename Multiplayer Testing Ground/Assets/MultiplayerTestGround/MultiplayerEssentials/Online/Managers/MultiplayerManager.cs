using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager Instance;

    [Header("Multiplayer Settings")]
    public int maxPlayers = 4;
    public string multiplayerSceneName = "TestOnlineMultiplayer"; // Doit être EXACT dans Build Settings

    private async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        await AuthManager.Instance.WaitForSignInAsync();
        Debug.Log("Authentification terminée. Prêt pour multijoueur.");
    }

    public async Task HostGame()
    {
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Join Code : " + joinCode);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "dtls"));

            if (!NetworkManager.Singleton.StartHost())
            {
                Debug.LogError("Erreur : StartHost a échoué.");
                return;
            }

            await LobbyService.Instance.CreateLobbyAsync("Lobby_" + Random.Range(1000, 9999), maxPlayers, new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    { "joinCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
                }
            });

            NetworkManager.Singleton.SceneManager.LoadScene(multiplayerSceneName, LoadSceneMode.Single);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Erreur HostGame : " + ex);
        }
    }

    public async Task JoinLastLobbyGame()
    {
        try
        {
            var lobbies = await LobbyService.Instance.QueryLobbiesAsync();

            if (lobbies.Results.Count == 0)
            {
                Debug.LogWarning("Aucun lobby trouvé.");
                return;
            }

            var lobby = lobbies.Results.Last();
            string joinCode = lobby.Data["joinCode"].Value;

            var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "dtls"));

            NetworkManager.Singleton.StartClient();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Erreur JoinLastLobbyGame : " + ex);
        }
    }
}
