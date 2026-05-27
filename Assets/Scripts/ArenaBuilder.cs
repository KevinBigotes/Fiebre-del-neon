using UnityEngine;

public class ArenaBuilder : MonoBehaviour
{
    [Header("Configuración")]
    public int wallCount = 24;
    public float radius = 10f;
    public float wallHeight = 2f;
    public Material wallMaterial;

    private void Start()
    {
        // CORRECCIÓN DE COLISIÓN DE SUELO:
        // El suelo (Arena_Floor) es un cilindro 3D con un CapsuleCollider por defecto.
        // Al escalarlo a (20, 0.2, 20), el CapsuleCollider se deforma en una esfera de radio 10.
        // Esto hace que el jugador flote a Y = 10.5, caminando sobre un domo invisible arriba de las paredes y coleccionables.
        GameObject floor = GameObject.Find("Arena_Floor");
        if (floor != null)
        {
            Collider oldCollider = floor.GetComponent<Collider>();
            if (oldCollider != null && oldCollider is CapsuleCollider)
            {
                Destroy(oldCollider);
                BoxCollider box = floor.AddComponent<BoxCollider>();
                box.size = new Vector3(1f, 1f, 1f); // Escalado con el transform a 20 x 0.2 x 20
                Debug.Log("[ArenaBuilder] Suelo corregido: CapsuleCollider reemplazado por BoxCollider.");
            }
        }

        BuildArena();
    }

    private void BuildArena()
    {
        // Limpia hijos anteriores
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        float angleStep = 360f / wallCount;
        float wallWidth = 2f * Mathf.PI * radius / wallCount + 0.1f;

        for (int i = 0; i < wallCount; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, wallHeight / 2f, Mathf.Sin(angle) * radius);

            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = $"Wall_{i}";
            wall.transform.SetParent(transform);
            wall.transform.position = pos;
            wall.transform.rotation = Quaternion.Euler(0, -i * angleStep + 90f, 0);
            wall.transform.localScale = new Vector3(wallWidth, wallHeight, 0.4f);

            if (wallMaterial != null)
                wall.GetComponent<Renderer>().material = wallMaterial;
        }
    }
}