using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInteractable : MonoBehaviour, IPointerEnterHandler
{
    bool hovered = false;

    void OnEnable()
    {
        hovered = false;
    }

    void OnDisable()
    {
        hovered = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
    }
}