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
    public string multiplayerSceneName = "TestOnlineMultiplayer";

    private string currentJoinCode;

    // Event déclenché quand le join code est prêt
    public delegate void JoinCodeReadyHandler(string joinCode);
    public event JoinCodeReadyHandler OnJoinCodeReady;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Un autre MultiplayerManager existe déjà, destruction de celui-ci.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        float timeout = 5f, elapsed = 0f;
        while (AuthManager.Instance == null && elapsed < timeout)
        {
            await Task.Delay(100);
            elapsed += 0.1f;
        }
        if (AuthManager.Instance == null)
        {
            Debug.LogError("AuthManager toujours null après 5 secondes.");
            return;
        }

        await AuthManager.Instance.WaitForSignInAsync();
        Debug.Log("Authentification réussie.");
    }

    public string GetJoinCode()
    {
        Debug.Log($"GetJoinCode() appelé. Instance ID: {Instance.GetInstanceID()}, JoinCode: {currentJoinCode}");
        return currentJoinCode;
    }

    /// <summary>
    /// Héberge la partie, crée un lobby et retourne le join code.
    /// </summary>
    public async Task<string> HostGame()
    {
        try
        {
            Debug.Log("Démarrage du host...");

            var allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
            currentJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log("Join code généré : " + currentJoinCode);

            // Notifier que le code est prêt
            OnJoinCodeReady?.Invoke(currentJoinCode);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "dtls"));

            if (!NetworkManager.Singleton.StartHost())
            {
                Debug.LogError("Échec du démarrage du host");
                return null;
            }

            await LobbyService.Instance.CreateLobbyAsync(
                "Lobby_" + Random.Range(1000, 9999),
                maxPlayers,
                new CreateLobbyOptions
                {
                    IsPrivate = false,
                    Data = new Dictionary<string, DataObject>
                    {
                        { "joinCode", new DataObject(DataObject.VisibilityOptions.Member, currentJoinCode) }
                    }
                });

            NetworkManager.Singleton.SceneManager.LoadScene(multiplayerSceneName, LoadSceneMode.Single);

            return currentJoinCode;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Erreur dans HostGame : " + ex.Message);
            return null;
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

            var lastLobby = lobbies.Results.Last();
            var joinCode = lastLobby.Data["joinCode"].Value;

            Debug.Log("Join code récupéré : " + joinCode);

            var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "dtls"));

            NetworkManager.Singleton.StartClient();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Erreur dans JoinLastLobbyGame : " + ex.Message);
        }
    }
}
