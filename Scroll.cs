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

    private Bounds _bounds = new Bounds();
    public Bounds bounds { get { return _bounds; } }

    void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        _bounds = transform.GetComponent<RectTransform>().GetBounds(transform.rotation);

        _contentBounds = transform.GetComponent<RectTransform>().GetBoundsWithChildren(new List<GameObject>() { gameObject }, transform.rotation);
        if (!customScrollBounds)
        {
            int dimension = (int)scrollAlong;
            if (contentBounds.size[dimension] > bounds.size[dimension])
            {
                minScroll = -(contentBounds.max[dimension] - bounds.max[dimension]);
                maxScroll = -(contentBounds.min[dimension] - bounds.min[dimension]);
            }
            else
            {
                minScroll = -(contentBounds.min[dimension] - bounds.min[dimension]);
                maxScroll = -(contentBounds.max[dimension] - bounds.max[dimension]);
            }
        }
        ScrollBy(0f);
    }

    public void ScrollBy(float deltaScroll)
    {
        float deltaScrollOffset = -scrollOffset;
        scrollOffset += deltaScroll * 100f;
        scrollOffset = Math.Clamp(scrollOffset, minScroll, maxScroll);
        deltaScrollOffset += scrollOffset;

        int dimension = (int)scrollAlong;

        foreach (Transform child in transform)
        {
            child.localPosition = transform.localPosition.ChangeValue(dimension, child.localPosition[dimension] + deltaScrollOffset);
        }
    }
}