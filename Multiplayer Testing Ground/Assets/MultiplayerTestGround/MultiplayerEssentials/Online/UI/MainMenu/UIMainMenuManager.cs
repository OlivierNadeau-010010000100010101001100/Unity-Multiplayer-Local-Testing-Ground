using UnityEngine;

public class UIMainMenuManager : MonoBehaviour
{
    public MultiplayerManager multiplayerManager;

    public async void OnHostButtonClicked()
    {
        Debug.Log("Hébergement en cours...");
        await multiplayerManager.HostGame();
    }

    public async void OnJoinButtonClicked()
    {
        Debug.Log("Connexion à une partie en cours...");
        await multiplayerManager.JoinLastLobbyGame();
    }

    public void OnQuitGameClicked()
    {
        Debug.Log("Application fermée");

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }


}
