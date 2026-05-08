using Unity.Netcode;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    public GameObject gameManagerPrefab;

    private void Start()
    {
        // Suscribirse al evento de cuando el servidor inicia
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    private void OnServerStarted()
    {
        Debug.Log("Servidor iniciado - spawneando GameManager");

        if (gameManagerPrefab != null)
        {
            GameObject gm = Instantiate(gameManagerPrefab);
            gm.GetComponent<NetworkObject>().Spawn();
        }
        else
        {
            Debug.Log("Sin GameManager prefab - iniciando spawn directo");
            SpawnPlayerDirect();
        }
    }

    private void SpawnPlayerDirect()
    {
        // Busca el GameManager ya existente en escena y llama spawn
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            NetworkObject no = gm.GetComponent<NetworkObject>();
            if (no != null && !no.IsSpawned)
            {
                no.Spawn();
                Debug.Log("GameManager spawneado en red");
            }
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
    }
}