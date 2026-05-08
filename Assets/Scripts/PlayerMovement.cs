using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 8f;
    public bool isFrozen = false;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (isFrozen) { rb.linearVelocity = Vector3.zero; return; }

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

        rb.linearVelocity = new Vector3(h, 0f, v) * moveSpeed;
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