using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton _clientSingletonPrefab;
    [SerializeField] private HostSingleton _hostSingletonPrefab;

    private async void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        await LauchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null); 
    }

    private async Task LauchInMode(bool isDedicatedServer)
    {
        if(isDedicatedServer)
        {

        }
        else
        {
            var hostSingleton = Instantiate(_hostSingletonPrefab);
            hostSingleton.CreateHost();

            var clientSingleton = Instantiate(_clientSingletonPrefab);
            var autherticated = await clientSingleton.CreateClient();


            if(autherticated)
            {
                clientSingleton.GameManager.GotoMenu();
            }
        }
    }
}
