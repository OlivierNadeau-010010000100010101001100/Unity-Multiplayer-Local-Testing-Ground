using UnityEngine;

public class ToggleMenuUI : MonoBehaviour
{
    public static ToggleMenuUI Instance;

    public GameObject menuUI;

    private FirstPersonLook lookScript;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        SetMenuActive(false);
        SetCursorState(false);

        foreach (FirstPersonLook cam in Object.FindObjectsByType<FirstPersonLook>(FindObjectsSortMode.None))
        {
            if (cam.IsOwner)
            {
                lookScript = cam;
                break;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            bool isActive = !menuUI.activeSelf;
            SetMenuActive(isActive);
            SetCursorState(isActive);

            if (lookScript != null)
                lookScript.canLook = !isActive;
        }
    }

    public void SetCursorState(bool isMenuOpen)
    {
        Cursor.lockState = isMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isMenuOpen;
    }

    public void SetMenuActive(bool change)
    {
        menuUI.SetActive(change);
    }

    public static void SetMenuActiveStatic(bool change)
    {
        if (Instance != null)
        {
            Instance.SetMenuActive(change);
            Instance.SetCursorState(change);

            if (Instance.lookScript != null)
                Instance.lookScript.canLook = !change;
        }
    }
}
