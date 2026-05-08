using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    public int score = 0;
    public int consecutivePositive = 0;
    public bool hasExpertCollectorAchievement = false;

    public void AddPoints(int points)
    {
        score += points;
        if (score < 0) score = 0;

        if (points > 0)
        {
            consecutivePositive += points;
            if (consecutivePositive >= 50 && !hasExpertCollectorAchievement)
            {
                hasExpertCollectorAchievement = true;
                UIManager.Instance?.ShowAchievement("¡Recolector Experto!");
            }
        }
        else
        {
            consecutivePositive = 0;
        }

        Debug.Log($"Score actualizado: {score}");
    }
}