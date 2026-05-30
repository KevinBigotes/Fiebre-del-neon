using UnityEngine;

public enum CollectibleType { Positive, Negative, Freeze, MegaPositive }

public class Collectible : MonoBehaviour
{
    public CollectibleType type;
    public int pointValue = 5;
    public float freezeDuration = 3f;

    [Header("Efectos de Sonido (Opcional)")]
    public AudioClip customCollectSound;

    // Animación de flotación
    private float floatSpeed = 2f;
    private float floatHeight = 0.3f;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
        Debug.Log($"[Collectible] Creado {gameObject.name} en posición: {startPos}");

        // Garantizar un Rigidbody kinematic para que los triggers funcionen al mover el transform manualmente
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            Debug.Log($"[Collectible] Rigidbody añadido a {gameObject.name}");
        }
        rb.isKinematic = true;
        rb.useGravity = false;
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
        Debug.Log($"[Collectible] OnTriggerEnter en {gameObject.name} con: {other.gameObject.name} (en posición: {other.transform.position})");
        PlayerScore score = other.GetComponentInParent<PlayerScore>();
        PlayerMovement movement = other.GetComponentInParent<PlayerMovement>();

        if (score == null)
        {
            Debug.Log($"[Collectible] No se encontró PlayerScore en {other.gameObject.name}");
            return;
        }

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

        PlaySoundEffect(other.transform.position);
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
        tm.fontSize = 12; // Más chiquito (era 24)
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;

        // Añadir comportamiento de animación física y desvanecimiento
        textObj.AddComponent<FloatingTextBehaviour>();

        // Actualiza UI recalculando todo desde el UIManager
        UIManager.Instance?.UpdateScores();
    }

    private void PlaySoundEffect(Vector3 position)
    {
        AudioClip clipToPlay = customCollectSound;
        if (clipToPlay == null)
        {
            // Usar sonido sintetizado en memoria si no hay audio clip cargado
            clipToPlay = CreateProceduralSound(type);
        }

        if (clipToPlay != null)
        {
            AudioSource.PlayClipAtPoint(clipToPlay, position, 0.7f); // Reproducir a volumen moderado
        }
    }

    private static AudioClip CreateProceduralSound(CollectibleType type)
    {
        int sampleRate = 44100;
        float duration = 0.15f;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        switch (type)
        {
            case CollectibleType.Positive:
                // Sonido agudo ascendente (bip alegre de punto positivo)
                for (int i = 0; i < sampleCount; i++)
                {
                    float t = (float)i / sampleCount;
                    float freq = Mathf.Lerp(600f, 900f, t);
                    samples[i] = Mathf.Sin(2f * Mathf.PI * freq * i / sampleRate) * (1f - t);
                }
                break;
            case CollectibleType.Negative:
                // Sonido grave descendente (bip triste de punto negativo)
                duration = 0.25f;
                sampleCount = (int)(sampleRate * duration);
                samples = new float[sampleCount];
                for (int i = 0; i < sampleCount; i++)
                {
                    float t = (float)i / sampleCount;
                    float freq = Mathf.Lerp(300f, 150f, t);
                    samples[i] = Mathf.Sin(2f * Mathf.PI * freq * i / sampleRate) * (1f - t);
                }
                break;
            case CollectibleType.Freeze:
                // Sonido vibrante y metálico (efecto congelar)
                duration = 0.3f;
                sampleCount = (int)(sampleRate * duration);
                samples = new float[sampleCount];
                for (int i = 0; i < sampleCount; i++)
                {
                    float t = (float)i / sampleCount;
                    float freq = 440f + Mathf.Sin(t * 40f) * 120f;
                    samples[i] = Mathf.Sin(2f * Mathf.PI * freq * i / sampleRate) * (1f - t);
                }
                break;
            case CollectibleType.MegaPositive:
                // Fanfarria arpegiada rápida (efecto megapuntos)
                duration = 0.35f;
                sampleCount = (int)(sampleRate * duration);
                samples = new float[sampleCount];
                for (int i = 0; i < sampleCount; i++)
                {
                    float t = (float)i / sampleCount;
                    float freq = 523.25f; // Do5 (C5)
                    if (t > 0.66f) freq = 783.99f; // Sol5 (G5)
                    else if (t > 0.33f) freq = 659.25f; // Mi5 (E5)

                    samples[i] = Mathf.Sin(2f * Mathf.PI * freq * i / sampleRate) * (1f - t);
                }
                break;
        }

        AudioClip clip = AudioClip.Create($"Procedural_{type}", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}

// Script auxiliar para animar el texto flotante hacia arriba y desvanecerlo
public class FloatingTextBehaviour : MonoBehaviour
{
    public float speed = 1.8f;
    public float duration = 1.0f;

    private float timer = 0f;
    private TextMesh textMesh;
    private Color startColor;

    private void Start()
    {
        textMesh = GetComponent<TextMesh>();
        if (textMesh != null)
        {
            startColor = textMesh.color;
        }
        Destroy(gameObject, duration);
    }

    private void Update()
    {
        // Mover hacia arriba
        transform.Translate(Vector3.up * speed * Time.deltaTime);
        timer += Time.deltaTime;

        // Desvanecimiento suave (fade out)
        if (textMesh != null)
        {
            textMesh.color = Color.Lerp(startColor, new Color(startColor.r, startColor.g, startColor.b, 0f), timer / duration);
        }
    }
}