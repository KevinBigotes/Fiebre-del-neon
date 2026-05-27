using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Configuración de Seguimiento")]
    public Vector3 offset = new Vector3(0f, 8f, -9f); // Posición relativa al jugador para la perspectiva inclinada
    public float smoothSpeed = 0.125f;                // Velocidad de transición suave
    public Vector3 lookAtOffset = new Vector3(0f, 0.5f, 0f); // Desplazamiento del punto de enfoque

    private Transform target;

    private void LateUpdate()
    {
        // Si no hay un objetivo, intentamos encontrar al jugador
        if (target == null)
        {
            FindTargetPlayer();
            if (target == null) return;
        }

        // Calcular la posición objetivo deseada
        Vector3 desiredPosition = target.position + offset;
        
        // Suavizar el movimiento usando Lerp
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Apuntar la cámara al jugador con el offset de enfoque
        transform.LookAt(target.position + lookAtOffset);
    }

    private void FindTargetPlayer()
    {
        // Buscar jugadores con el script PlayerMovement en la escena
        PlayerMovement[] players = FindObjectsOfType<PlayerMovement>();
        foreach (var player in players)
        {
            // Si es Netcode, seguimos al local player
            var netObj = player.GetComponent<Unity.Netcode.NetworkObject>();
            if (netObj != null)
            {
                if (netObj.IsLocalPlayer)
                {
                    target = player.transform;
                    break;
                }
            }
            else
            {
                // Si es modo local (pruebas sin red), seguimos al primer jugador que encontremos
                target = player.transform;
                break;
            }
        }
    }
}
