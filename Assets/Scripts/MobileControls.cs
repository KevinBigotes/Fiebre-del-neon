using UnityEngine;
using UnityEngine.EventSystems;

public class MobileControls : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum ButtonType { Left, Right, Up, Down, Dash }
    public ButtonType buttonType;

    private static Vector2 moveInput = Vector2.zero;
    private static bool dashPressed = false;

    public static Vector2 GetMoveInput() => moveInput;
    public static bool GetDash()
    {
        if (dashPressed) { dashPressed = false; return true; }
        return false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        switch (buttonType)
        {
            case ButtonType.Left:  moveInput.x = -1; break;
            case ButtonType.Right: moveInput.x =  1; break;
            case ButtonType.Up:    moveInput.y =  1; break;
            case ButtonType.Down:  moveInput.y = -1; break;
            case ButtonType.Dash:  dashPressed = true; break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        switch (buttonType)
        {
            case ButtonType.Left:
            case ButtonType.Right: moveInput.x = 0; break;
            case ButtonType.Up:
            case ButtonType.Down:  moveInput.y = 0; break;
        }
    }
}