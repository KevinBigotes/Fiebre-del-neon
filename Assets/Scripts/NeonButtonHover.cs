using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NeonButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    private Image bg;
    private Color originalColor;
    
    void Start()
    {
        originalScale = transform.localScale;
        bg = GetComponent<Image>();
        if (bg != null) originalColor = bg.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = originalScale * 1.1f;
        if (bg != null) bg.color = new Color(0.2f, 0.0f, 0.4f, 1f); // Resalta en morado oscuro
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
        if (bg != null) bg.color = originalColor;
    }
}
