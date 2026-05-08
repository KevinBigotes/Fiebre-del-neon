using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour  // <-- MonoBehaviour, NO NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject playerPrefab;
    public GameObject positiveCollectiblePrefab;
    public GameObject negativeCollectiblePrefab;
    public GameObject freezeCollectiblePrefab;
    public GameObject megaCollectiblePrefab;
    public float arenaRadius = 9f;
    public float gameDuration = 75f;

    private float timeRemaining;
    private bool gameActive = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        Debug.Log("GameManager Start");
        timeRemaining = gameDuration;
        StartCoroutine(WaitForNetworkAndSpawn());
    }

    private IEnumerator WaitForNetworkAndSpawn()
    {
        Debug.Log("Esperando NetworkManager...");

        // Espera hasta que NetworkManager esté activo y sea host/server
        float timeout = 10f;
        while (timeout > 0)
        {
            if (NetworkManager.Singleton != null && 
                (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer))
            {
                Debug.Log("NetworkManager listo!");
                break;
            }
            timeout -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsHost)
        {
            Debug.LogError("No hay host activo. Spawneando localmente para prueba...");
            SpawnPlayerLocal();
            yield break;
        }

        yield return new WaitForSeconds(0.5f);
        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("PLAYER PREFAB ES NULL");
            return;
        }

        Debug.Log($"Spawneando jugadores. Clientes: {NetworkManager.Singleton.ConnectedClientsList.Count}");

        int i = 0;
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            Vector3 pos = new Vector3(i * 4f, 1f, 0f);
            GameObject go = Instantiate(playerPrefab, pos, Quaternion.identity);
            NetworkObject no = go.GetComponent<NetworkObject>();
            if (no != null)
                no.SpawnAsPlayerObject(client.ClientId, true);
            Debug.Log($"Jugador {i} spawneado en {pos}");
            i++;
        }

        gameActive = true;
        StartCoroutine(GameLoop());
    }

    // Para probar sin red
    private void SpawnPlayerLocal()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("PLAYER PREFAB ES NULL");
            return;
        }
        Debug.Log("Spawn local (sin red)");
        Instantiate(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        gameActive = true;
        StartCoroutine(GameLoop());
    }

   private IEnumerator GameLoop()
{
    // Empieza a spawnear objetos
    StartCoroutine(SpawnCollectiblesLoop());

    while (timeRemaining > 0)
    {
        yield return new WaitForSeconds(1f);
        timeRemaining -= 1f;
        UIManager.Instance?.UpdateTimer(timeRemaining);
    }

    gameActive = false;
    Debug.Log("Partida terminada");
    UIManager.Instance?.ShowEndScreen();
}

private IEnumerator SpawnCollectiblesLoop()
{
    while (gameActive)
    {
        SpawnRandomCollectible();
        yield return new WaitForSeconds(2f);
    }
}

private void SpawnRandomCollectible()
{
    // Elige prefab según probabilidad
    GameObject prefab;
    int rand = Random.Range(0, 100);

    if (rand < 50)      prefab = positiveCollectiblePrefab;
    else if (rand < 75) prefab = negativeCollectiblePrefab;
    else if (rand < 90) prefab = freezeCollectiblePrefab;
    else                prefab = megaCollectiblePrefab;

    if (prefab == null) return;

    // Posición aleatoria dentro de la arena
    float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
    float radius = Random.Range(1f, arenaRadius - 1.5f);
    Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, 0.5f, Mathf.Sin(angle) * radius);

    Instantiate(prefab, pos, Quaternion.identity);
}
}