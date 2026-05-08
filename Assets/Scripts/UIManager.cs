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
        endScreenPanel.SetActive(true);
        // Aquí puedes mostrar el ganador consultando los scores
        endScreenText.text = "¡TIEMPO AGOTADO!\n¡Fin de la partida!";
    }
}