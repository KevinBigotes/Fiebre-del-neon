using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Referencias")]
    public RectTransform background;
    public RectTransform handle;

    [Header("Configuración")]
    public float handleRange = 1f;

    private Vector2 input = Vector2.zero;
    private Vector2 center;

    public static Vector2 Direction { get; private set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        center = RectTransformUtility.WorldToScreenPoint(null, background.position);
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, eventData.position, eventData.pressEventCamera, out pos);

        float radius = background.sizeDelta.x * 0.5f;
        pos = Vector2.ClampMagnitude(pos, radius);
        handle.localPosition = pos;

        input = pos / radius;
        Direction = input;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
        Direction = Vector2.zero;
        handle.localPosition = Vector2.zero;
    }
}