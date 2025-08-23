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
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                await UnityServices.InitializeAsync();
            }

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
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
