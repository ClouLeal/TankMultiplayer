using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
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
        AuthState = AuthStates.Authenticating;
        var tries = 0;
        while (AuthState == AuthStates.Authenticating &&  tries < maxTries)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
            {
                AuthState = AuthStates.Authenticated;
                break;
            }

            tries++;
            await Task.Delay(1000);
        }
        return AuthState;
    }
}
