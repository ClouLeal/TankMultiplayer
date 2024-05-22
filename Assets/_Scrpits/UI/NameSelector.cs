using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameSelector : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private Button connectBtn;
    [SerializeField] private int minLenghName = 1;
    [SerializeField] private int maxLenghName = 12;

    public static string PlayerNameKey = "PlayerName";

    private void Start()
    {
        // This check if this is sever only
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        }

        nameField.text = PlayerPrefs.GetString(PlayerNameKey, string.Empty);
        OnChangePlayerName();
    }

    public void OnChangePlayerName()
    {
        connectBtn.interactable = nameField.text.Length >= minLenghName && nameField.text.Length <= maxLenghName;
    }

    public  void Connect()
    {
        PlayerPrefs.SetString(PlayerNameKey, nameField.text);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }
}
