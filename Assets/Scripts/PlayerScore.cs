using UnityEngine;
using Unity.Netcode;

public class PlayerScore : NetworkBehaviour
{
    public NetworkVariable<int> score = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> consecutivePositive = new NetworkVariable<int>(0);
    public NetworkVariable<bool> hasExpertCollectorAchievement = new NetworkVariable<bool>(false);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        // Cuando cambie el score en la red, actualizamos la UI localmente
        score.OnValueChanged += (int previousValue, int newValue) =>
        {
            UIManager.Instance?.UpdateScores();
        };
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        score.OnValueChanged -= (int previousValue, int newValue) => { };
    }

    public void AddPoints(int points)
    {
        if (!IsServer) return; // Solo el servidor puede sumar puntos

        score.Value += points;
        if (score.Value < 0) score.Value = 0;

        if (points > 0)
        {
            consecutivePositive.Value += points;
            if (consecutivePositive.Value >= 50 && !hasExpertCollectorAchievement.Value)
            {
                hasExpertCollectorAchievement.Value = true;
                // Mostrar logro (RPC al cliente si se desea, pero por ahora en Server)
                ShowAchievementClientRpc("¡Recolector Experto!");
            }
        }
        else
        {
            consecutivePositive.Value = 0;
        }

        Debug.Log($"Score actualizado en Servidor: {score.Value}");
    }

    [ClientRpc]
    private void ShowAchievementClientRpc(string achievementName)
    {
        UIManager.Instance?.ShowAchievement(achievementName);
    }
}