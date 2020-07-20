using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FollowPointer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private bool pointerIn = false, pointerDown = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerDown = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerIn = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerIn = false;
    }

    // Update is called once per frame
    void Update()
    {
        bool follow = pointerIn && pointerDown;
        if (follow)
        {
            Boid.Target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        Boid.FollowTarget = pointerIn && pointerDown;
    }
}
