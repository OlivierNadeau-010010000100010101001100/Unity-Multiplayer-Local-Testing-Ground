using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI joinCodeText;

    private void OnEnable()
    {
        if (MultiplayerManager.Instance != null)
        {
            MultiplayerManager.Instance.OnJoinCodeReady += UpdateJoinCodeText;
        }
    }

    private void OnDisable()
    {
        if (MultiplayerManager.Instance != null)
        {
            MultiplayerManager.Instance.OnJoinCodeReady -= UpdateJoinCodeText;
        }
    }

    private void Start()
    {
        if (MultiplayerManager.Instance != null)
        {
            string code = MultiplayerManager.Instance.GetJoinCode();
            if (!string.IsNullOrEmpty(code))
            {
                joinCodeText.text = code;
            }
        }
    }

    private void UpdateJoinCodeText(string joinCode)
    {
        joinCodeText.text = joinCode;
        Debug.Log("Join code mis à jour via event : " + joinCode);
    }

    public void OnLeaveButtonClicked()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu");
    }
}
