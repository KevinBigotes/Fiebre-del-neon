using UnityEngine;

public enum CollectibleType { Positive, Negative, Freeze, MegaPositive }

public class Collectible : MonoBehaviour
{
    public CollectibleType type;
    public int pointValue = 5;
    public float freezeDuration = 3f;

    // Animación de flotación
    private float floatSpeed = 2f;
    private float floatHeight = 0.3f;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        // Efecto de flotación
        float y = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, y, transform.position.z);

        // Rotación continua
        transform.Rotate(Vector3.up, 90f * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerScore score = other.GetComponent<PlayerScore>();
        PlayerMovement movement = other.GetComponent<PlayerMovement>();

        if (score == null) return;

        switch (type)
        {
            case CollectibleType.Positive:
                score.AddPoints(pointValue);
                ShowFloatingText($"+{pointValue}", Color.yellow, other.transform.position);
                break;
            case CollectibleType.Negative:
                score.AddPoints(-pointValue);
                ShowFloatingText($"-{pointValue}", Color.red, other.transform.position);
                break;
            case CollectibleType.Freeze:
                movement?.Freeze(freezeDuration);
                ShowFloatingText("¡CONGELADO!", Color.cyan, other.transform.position);
                break;
            case CollectibleType.MegaPositive:
                score.AddPoints(pointValue * 3);
                ShowFloatingText($"+{pointValue * 3}!", Color.magenta, other.transform.position);
                break;
        }

        Destroy(gameObject);
    }

    private void ShowFloatingText(string text, Color color, Vector3 pos)
    {
        // Crea texto flotante simple
        GameObject textObj = new GameObject("FloatingText");
        textObj.transform.position = pos + Vector3.up * 1.5f;

        var tm = textObj.AddComponent<TextMesh>();
        tm.text = text;
        tm.color = color;
        tm.fontSize = 24;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;

        Destroy(textObj, 1.5f);

        // Actualiza UI
        UIManager.Instance?.UpdateScores(
            FindObjectOfType<PlayerScore>()?.score ?? 0, 0);
    }
}