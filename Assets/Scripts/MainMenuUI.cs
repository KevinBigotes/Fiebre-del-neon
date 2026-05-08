using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    public TMP_InputField ipInputField;

    public void OnHostButton()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager no encontrado en la escena");
            return;
        }

        bool started = NetworkManager.Singleton.StartHost();

        if (started)
        {
            Debug.Log("Host iniciado correctamente");
            // Carga la escena de juego
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            Debug.LogError("No se pudo iniciar el Host");
        }
    }

    public void OnJoinButton()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager no encontrado en la escena");
            return;
        }

        string ip = "127.0.0.1";
        if (ipInputField != null && !string.IsNullOrEmpty(ipInputField.text))
            ip = ipInputField.text;

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport != null)
            transport.SetConnectionData(ip, 7777);

        NetworkManager.Singleton.StartClient();
        SceneManager.LoadScene("GameScene");
    }
}