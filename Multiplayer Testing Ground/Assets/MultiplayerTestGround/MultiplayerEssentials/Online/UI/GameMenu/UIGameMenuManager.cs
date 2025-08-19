using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public void OnLeaveButtonClicked()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu");
    }
}
