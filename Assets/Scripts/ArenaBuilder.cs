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
            wall.transform.rotation = Quaternion.Euler(0, i * angleStep + 90f, 0);
            wall.transform.localScale = new Vector3(wallWidth, wallHeight, 0.4f);

            if (wallMaterial != null)
                wall.GetComponent<Renderer>().material = wallMaterial;
        }
    }
}