using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager
{

    private const string MenuSceneName = "Menu";
    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync();

        var authState = await AuthenticationWrapper.DoAuth();

        if(authState == AuthStates.Authenticated)
        {
            return true;
        }

        return false;
    }

    internal void GotoMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }
}
