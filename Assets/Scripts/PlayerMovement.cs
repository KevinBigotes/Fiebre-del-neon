using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 8f;
    public bool isFrozen = false;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            Debug.Log($"[PlayerMovement] Awake: Rigidbody encontrado. useGravity set to true. Constraints: {rb.constraints}");
        }
        else
        {
            Debug.LogError("[PlayerMovement] Awake: ¡No se encontró Rigidbody en el objeto!");
        }
    }

    private void Start()
    {
        Debug.Log($"[PlayerMovement] Start: Jugador creado en posición: {transform.position}");
    }

    private int logCounter = 0;

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }
        if (isFrozen) { rb.linearVelocity = Vector3.zero; return; }

        // Registrar posición periódicamente para debugear la altura
        logCounter++;
        if (logCounter % 60 == 0)
        {
            Debug.Log($"[PlayerMovement] Posición actual del jugador: {transform.position}, Velocidad: {rb.linearVelocity}");
        }

        // Joystick táctil
        Vector2 joy = VirtualJoystick.Direction;

        // Teclado como respaldo (PC)
        float h = joy.x;
        float v = joy.y;

        if (Mathf.Abs(h) < 0.01f && Mathf.Abs(v) < 0.01f)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))  h = -1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) h =  1f;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))    v =  1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))  v = -1f;
        }

        rb.linearVelocity = new Vector3(h * moveSpeed, rb.linearVelocity.y, v * moveSpeed);
    }

    public void Freeze(float duration)
    {
        StartCoroutine(FreezeCoroutine(duration));
    }

    private System.Collections.IEnumerator FreezeCoroutine(float duration)
    {
        isFrozen = true;
        yield return new WaitForSeconds(duration);
        isFrozen = false;
    }
}