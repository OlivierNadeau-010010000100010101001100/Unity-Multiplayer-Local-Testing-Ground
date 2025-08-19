using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading;
using System.Threading.Tasks;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance;

    private SemaphoreSlim signInLock = new SemaphoreSlim(1, 1);
    private bool isSignedIn = false;

    async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            await InitializeAndSignIn();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async Task InitializeAndSignIn()
    {
        await signInLock.WaitAsync();
        try
        {
            // ✅ Appeler UnityServices.InitializeAsync() ici AVANT AuthenticationService.Instance
            if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
            {
                await UnityServices.InitializeAsync();
            }

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("✅ Sign-in réussi : " + AuthenticationService.Instance.PlayerId);
            }
            else
            {
                Debug.Log("⚠️ Déjà signé : " + AuthenticationService.Instance.PlayerId);
            }

            isSignedIn = true;
        }
        finally
        {
            signInLock.Release();
        }
    }

    public async Task WaitForSignInAsync()
    {
        while (!isSignedIn)
        {
            await Task.Delay(100);
        }
    }
}
