using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;

    [Header("Paneles")]
    public GameObject achievementPanel;
    public TextMeshProUGUI achievementText;
    public GameObject megaAlertPanel;
    public GameObject endScreenPanel;
    public TextMeshProUGUI endScreenText;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void UpdateTimer(float seconds)
    {
        int mins = Mathf.FloorToInt(seconds / 60);
        int secs = Mathf.FloorToInt(seconds % 60);
        timerText.text = $"TIEMPO: {mins:00}:{secs:00}";

        // Parpadeo rojo en los últimos 15 segundos
        if (seconds <= 15f)
            timerText.color = Color.red;
    }

    public void UpdateScores(int p1Score, int p2Score)
    {
        player1ScoreText.text = $"PUNTOS (J1): {p1Score}";
        player2ScoreText.text = $"PUNTOS (J2): {p2Score}";
    }

    public void ShowAchievement(string name)
    {
        StartCoroutine(ShowAchievementCoroutine(name));
    }

    private IEnumerator ShowAchievementCoroutine(string name)
    {
        achievementText.text = $"🏆 LOGRO: {name}";
        achievementPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        achievementPanel.SetActive(false);
    }

    public void ShowMegaAlert()
    {
        StartCoroutine(MegaAlertCoroutine());
    }

    private IEnumerator MegaAlertCoroutine()
    {
        megaAlertPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        megaAlertPanel.SetActive(false);
    }

    public void ShowEndScreen()
    {
        try
        {
            endScreenPanel.SetActive(true);
            // Ajustar el panel de fondo para que cubra la pantalla y sea oscuro
            UnityEngine.UI.Image bgImage = endScreenPanel.GetComponent<UnityEngine.UI.Image>();
            if (bgImage != null)
            {
                bgImage.color = new Color(0.05f, 0.0f, 0.1f, 0.95f); // Morado casi negro
            }

            RectTransform panelRT = endScreenPanel.GetComponent<RectTransform>();
            if (panelRT != null)
            {
                panelRT.anchorMin = Vector2.zero;
                panelRT.anchorMax = Vector2.one;
                panelRT.offsetMin = Vector2.zero;
                panelRT.offsetMax = Vector2.zero;
            }

            // Ajustar el cuadro de texto para que sea ancho y no apriete las letras
            if (endScreenText != null) 
            {
                endScreenText.gameObject.SetActive(true);
                RectTransform textRT = endScreenText.GetComponent<RectTransform>();
                if (textRT != null)
                {
                    textRT.anchorMin = new Vector2(0.05f, 0.1f);
                    textRT.anchorMax = new Vector2(0.95f, 0.9f);
                    textRT.offsetMin = Vector2.zero;
                    textRT.offsetMax = Vector2.zero;
                }
                
                // Alineación y estilo
                endScreenText.alignment = TextAlignmentOptions.Center;
                endScreenText.richText = true;
                
                // Sombra neón para el texto si no tiene
                UnityEngine.UI.Shadow shadow = endScreenText.gameObject.GetComponent<UnityEngine.UI.Shadow>();
                if (shadow == null) shadow = endScreenText.gameObject.AddComponent<UnityEngine.UI.Shadow>();
                shadow.effectColor = new Color(0f, 1f, 1f, 0.5f);
                shadow.effectDistance = new Vector2(4, -4);
            }
            
            PlayerScore[] players = FindObjectsOfType<PlayerScore>();
            int hostScore = 0;
            int clientScore = 0;
            
            foreach (var p in players)
            {
                if (p == null) continue;
                var no = p.GetComponent<Unity.Netcode.NetworkObject>();
                if (no != null && no.IsSpawned)
                {
                    if (no.OwnerClientId == 0) hostScore = p.score;
                    else clientScore = p.score;
                }
                else
                {
                    // Partida local (sin red activada)
                    hostScore = p.score; 
                }
            }
            
            string result = "<color=#00FFFF>¡EMPATE!</color>";
            if (hostScore > clientScore) result = "<color=#00FFFF>¡EL HOST GANA!</color>";
            else if (clientScore > hostScore) result = "<color=#00FFFF>¡EL CLIENTE GANA!</color>";

            endScreenText.text = $"<size=120><color=#FF00FF><b>¡TIEMPO AGOTADO!</b></color></size>\n\n<size=80>{result}</size>\n\n<size=60>Host: {hostScore} pts</size>\n<size=60>Cliente: {clientScore} pts</size>";
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error in ShowEndScreen: " + e.Message);
            if (endScreenText != null)
            {
                endScreenText.color = Color.red;
                endScreenText.text = "ERROR: " + e.Message;
            }
        }
    }
}