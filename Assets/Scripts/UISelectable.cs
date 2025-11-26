using UnityEngine;
using UnityEngine.EventSystems;

public class UISelectable : MonoBehaviour
{
    public int index;
    public Vector2 GetScreenPosition()
    {
        RectTransform rect = GetComponent<RectTransform>();
        Vector3 worldCenter = rect.TransformPoint(rect.rect.center);

        Canvas canvas = GetComponentInParent<Canvas>();
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;

        return RectTransformUtility.WorldToScreenPoint(cam, worldCenter);
    }


    public void OnSubmit()
    {
        Debug.Log("Submit on slot " + index);

        // Crée un PointerEventData pour simuler un clic
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute<IPointerClickHandler>(
            gameObject,
            pointer,
            ExecuteEvents.pointerClickHandler
        );
    }
}
