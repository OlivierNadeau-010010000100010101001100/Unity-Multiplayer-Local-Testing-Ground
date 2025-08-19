using UnityEngine;

public class UIMainMenu : MonoBehaviour
{
    public GameObject menuUI;

    void Start()
    {
        // Le menu principal est visible au lancement, on s'assure que le curseur est visible
        menuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowMenu(bool show)
    {
        menuUI.SetActive(show);
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = show;
    }
}
