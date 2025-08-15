using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public MultiplayerManager multiplayerManager;
    public TMP_InputField joinCodeInput;

    public TMP_Text joinCodeDisplay; // 🆕 Le texte qui affichera le code

    public async void OnHostButtonClicked()
    {
        string code = await multiplayerManager.HostGame();
        Debug.Log("Host lancé avec code : " + code);

        // Afficher le code dans le texte UI
        joinCodeDisplay.text = "Join Code : " + code;
    }

    public async void OnJoinButtonClicked()
    {
        string code = joinCodeInput.text;
        if (!string.IsNullOrEmpty(code))
        {
            await multiplayerManager.JoinGame(code);
            Debug.Log("Client connecté avec le code : " + code);
        }
        else
        {
            Debug.LogWarning("Merci d'entrer un code valide.");
        }
    }
}
