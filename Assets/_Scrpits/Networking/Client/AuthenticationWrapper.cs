using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
public enum AuthStates
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut,
}
public static class AuthenticationWrapper 
{
   public static AuthStates AuthState {  get; private set; } = AuthStates.NotAuthenticated;

    public static async Task<AuthStates> DoAuth(int maxTries = 5)
    {
        if (AuthState == AuthStates.Authenticated)
        {
            return AuthState;
        }

        if(AuthState == AuthStates.Authenticating) 
        {
            Debug.LogWarning("Alread authernticating");
            await Authenticating();
            return AuthState;
        }

        await SignInAnonymouslyAsync(maxTries);

        return AuthState;
    }

    private static async Task<AuthStates> Authenticating()
    {
        while (AuthState == AuthStates.Authenticating || AuthState == AuthStates.NotAuthenticated)
        {
            await Task.Delay(200);
        }

        return AuthState;
    }

    private  static  async Task SignInAnonymouslyAsync(int maxTries)
    {
        AuthState = AuthStates.Authenticating;

        var tries = 0;
        while (AuthState == AuthStates.Authenticating && tries < maxTries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthState = AuthStates.Authenticated;
                    break;
                }
            }
            catch (AuthenticationException ex)
            {
                Debug.LogException(ex);
                AuthState = AuthStates.Error;
            }
            catch( RequestFailedException rfe )
            {
                Debug.LogException(rfe);
                AuthState = AuthStates.Error;
            }
           
            tries++;
            await Task.Delay(1000);

            if(AuthState != AuthStates.Authenticating)
            {
                Debug.LogWarning($"Player was not signed in successfully after {tries} tries");
                AuthState = AuthStates.TimeOut;
            }
        }
       
    }
}
