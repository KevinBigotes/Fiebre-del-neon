using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    public TMP_InputField ipInputField;

    private void Start()
    {
        ApplyNeonTheme();
    }

    private void ApplyNeonTheme()
    {
        // 1. Fondo oscuro
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.backgroundColor = new Color(0.02f, 0.02f, 0.08f);
            mainCam.clearFlags = CameraClearFlags.SolidColor;
        }

        // 2. Estilizar Títulos
        foreach (var txt in FindObjectsOfType<TextMeshProUGUI>())
        {
            if (txt.text.ToUpper().Contains("FIEBRE DEL NEÓN"))
            {
                txt.color = new Color(0f, 1f, 1f); // Cyan
                txt.fontStyle = FontStyles.Bold;
                UnityEngine.UI.Shadow shadow = txt.gameObject.AddComponent<UnityEngine.UI.Shadow>();
                shadow.effectColor = new Color(0f, 0.5f, 1f, 0.8f);
                shadow.effectDistance = new Vector2(3, -3);

                RectTransform rt = txt.GetComponent<RectTransform>();
                if (rt != null) {
                    rt.anchoredPosition += new Vector2(0, 300f);
                }
            }
        }

        // 3. Estilizar Botones
        foreach (var btn in FindObjectsOfType<UnityEngine.UI.Button>())
        {
            UnityEngine.UI.Image bg = btn.GetComponent<UnityEngine.UI.Image>();
            if (bg != null)
            {
                bg.color = new Color(0.1f, 0.1f, 0.2f, 0.9f);
                UnityEngine.UI.Outline outline = btn.gameObject.AddComponent<UnityEngine.UI.Outline>();
                outline.effectColor = new Color(1f, 0f, 1f, 0.8f); // Magenta outline
                outline.effectDistance = new Vector2(4, -4);
            }
            
            TextMeshProUGUI btnTxt = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (btnTxt != null)
            {
                btnTxt.color = Color.white;
                btnTxt.fontStyle = FontStyles.Bold;
            }

            if (btn.gameObject.GetComponent<NeonButtonHover>() == null)
            {
                btn.gameObject.AddComponent<NeonButtonHover>();
            }
        }

        // 4. Estilizar Input Field
        if (ipInputField != null)
        {
            UnityEngine.UI.Image bg = ipInputField.GetComponent<UnityEngine.UI.Image>();
            if (bg != null)
            {
                bg.color = new Color(0.05f, 0.05f, 0.1f, 1f);
                UnityEngine.UI.Outline outline = ipInputField.gameObject.AddComponent<UnityEngine.UI.Outline>();
                outline.effectColor = new Color(0f, 1f, 1f, 0.8f); // Cyan outline
                outline.effectDistance = new Vector2(2, -2);
            }
            if (ipInputField.textComponent != null)
            {
                ipInputField.textComponent.color = Color.white;
            }
        }
    }

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
            NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
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
    }
}