using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoytickController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Image joystickBackgroundImage;
    public Image joystickImage;
    private Vector2 posInput;

    public void OnDrag(PointerEventData evenData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackgroundImage.rectTransform,
            evenData.position,
            evenData.pressEventCamera,
            out posInput))
        {
            posInput.x = posInput.x / (joystickBackgroundImage.rectTransform.sizeDelta.x);
            posInput.y = posInput.y / (joystickBackgroundImage.rectTransform.sizeDelta.y);

            if (posInput.magnitude > 1f)
            {
                posInput = posInput.normalized;
            }

            joystickImage.rectTransform.anchoredPosition = new Vector2(
                posInput.x * (joystickBackgroundImage.rectTransform.sizeDelta.x / 2),
                posInput.y * (joystickBackgroundImage.rectTransform.sizeDelta.x / 2));
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        posInput = Vector2.zero;
        joystickImage.rectTransform.anchoredPosition = Vector2.zero;
    }

    public float inputHorizontal()
    {
        if (posInput.x != 0)
        {
            return posInput.x;
        }
        else
        {
            return Input.GetAxis(Constant.HORIZONTAL_AXIS);
        }
    }

    public float inputVertical()
    {
        if (posInput.y != 0)
        {
            return posInput.y;
        }
        else return Input.GetAxis(Constant.VERTICAL_AXIS);
    }
}
