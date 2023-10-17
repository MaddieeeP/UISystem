using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll : MonoBehaviour
{
    public Dimension scrollAlong = Dimension.y;
    public bool customScrollBounds = false;

    public float maxScroll = 0f;
    public float minScroll = 0f;
    private float scrollOffset = 0f;

    private Bounds _contentBounds = new Bounds();
    public Bounds contentBounds { get { return _contentBounds; } }

    private Vector2 _size = Vector2.zero;
    public Vector2 size { get { return _size; } }

    void OnEnable()
    {
        SetContentBounds();

        if (!customScrollBounds)
        {
            minScroll = contentBounds.min[(int)scrollAlong];
            maxScroll = contentBounds.max[(int)scrollAlong];
        }
    }

    void SetContentBounds()
    {
        _contentBounds = transform.GetComponent<RectTransform>().GetBoundsWithChildren(new List<GameObject>() { gameObject }, transform.rotation);
    }

    public void ScrollBy(float deltaScroll)
    {
        scrollOffset += deltaScroll;
        scrollOffset = Math.Clamp(scrollOffset, minScroll, maxScroll);
        //transform.localPosition = Vector2.Lerp();
    }
}
