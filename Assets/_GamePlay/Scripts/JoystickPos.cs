using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickPos : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Canvas canvas;
    public Image joystickBackgroundImage;
    public Image joystickImage;

    public JoytickController joystickController;

    private void Start()
    {
        joystickBackgroundImage.enabled = false;
        joystickImage.enabled = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PointerEventData pointerData = eventData;

        Vector2 position;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            pointerData.position,
            canvas.worldCamera,
            out position);

        joystickBackgroundImage.transform.position = canvas.transform.TransformPoint(position);

        joystickBackgroundImage.enabled = true;
        joystickImage.enabled = true;

    }

    public void OnDrag(PointerEventData eventData)
    {
        joystickController.OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        joystickController.OnPointerUp(eventData);
        joystickBackgroundImage.enabled = false;
        joystickImage.enabled = false;
    }
}

