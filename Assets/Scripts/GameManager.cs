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
    public bool isGameActive { get; private set; } = false;
    public bool isGameOver { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        Debug.Log("GameManager Start");
        timeRemaining = gameDuration;

        // Adjuntar el componente de seguimiento de cámara a la cámara principal en tiempo de ejecución
        Camera mainCam = Camera.main;
        if (mainCam != null && mainCam.gameObject.GetComponent<CameraFollow>() == null)
        {
            mainCam.gameObject.AddComponent<CameraFollow>();
            Debug.Log("[GameManager] Script CameraFollow añadido con éxito a la cámara.");
        }

        StartCoroutine(WaitForNetworkAndSpawn());
    }

    private IEnumerator WaitForNetworkAndSpawn()
    {
        Debug.Log("Esperando NetworkManager...");

        // Si no hay NetworkManager o no hay una sesión activa, jugamos localmente de inmediato (0 segundos de espera)
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
        {
            Debug.Log("[GameManager] No hay sesión de red activa. Iniciando partida local inmediatamente...");
            SpawnPlayerLocal();
            yield break;
        }

        // Si hay una sesión activa como Servidor o Host, spawneamos a los jugadores
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
        {
            Debug.Log("[GameManager] Sesión de red activa como Servidor/Host. Spawneando jugadores en red...");
            yield return new WaitForSeconds(0.2f);
            SpawnPlayers();
        }
        else // Si es un Cliente
        {
            Debug.Log("[GameManager] Sesión de red activa como Cliente. Esperando a que el Servidor cree los objetos...");
            isGameActive = true;
            StartCoroutine(GameLoop());
        }
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
            if (client.PlayerObject != null)
            {
                client.PlayerObject.transform.position = pos;
                Debug.Log($"Jugador {i} reubicado en {pos}");
            }
            else if (playerPrefab != null)
            {
                GameObject go = Instantiate(playerPrefab, pos, Quaternion.identity);
                NetworkObject no = go.GetComponent<NetworkObject>();
                if (no != null)
                    no.SpawnAsPlayerObject(client.ClientId, true);
                Debug.Log($"Jugador {i} spawneado en {pos}");
            }
            i++;
        }

        isGameActive = true;
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
        isGameActive = true;
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

    isGameActive = false;
    isGameOver = true;
    Debug.Log("Partida terminada");
    UIManager.Instance?.ShowEndScreen();

    // Esperar 5 segundos y regresar al menú
    yield return new WaitForSeconds(5f);

    if (NetworkManager.Singleton != null)
    {
        NetworkManager.Singleton.Shutdown();
    }
    UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
}

    private IEnumerator SpawnCollectiblesLoop()
    {
        while (isGameActive)
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

    if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer) return;

    // Posición aleatoria dentro de la arena
    float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
    float radius = Random.Range(1f, arenaRadius - 1.5f);
    Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, 0.5f, Mathf.Sin(angle) * radius);

    GameObject go = Instantiate(prefab, pos, Quaternion.identity);
    var no = go.GetComponent<Unity.Netcode.NetworkObject>();
    if (no != null)
    {
        no.Spawn();
    }
}
}