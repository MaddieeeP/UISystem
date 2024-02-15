using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll : MonoBehaviour, IUIElement
{
    [SerializeField] private Dimension _scrollAlong = Dimension.y;
    [SerializeField] private float _scrollSpeed = 100f;
    [SerializeField] private bool _keepOffsetOnInitialize = true;
    [SerializeField] private int _initializeAlign = 0;

    private float _maxScroll = 0f;
    private float _minScroll = 0f;
    private Bounds _contentBounds = new Bounds();
    private Bounds _bounds = new Bounds();

    //getters and setters
    public Dimension scrollAlong { get { return _scrollAlong; } }
    public bool keepOffsetOnInitialize { get { return _keepOffsetOnInitialize; } }
    public int initializeAlign { get { return _initializeAlign; } set { _initializeAlign = value; } }
    public Bounds contentBounds { get { return _contentBounds; } }
    public Bounds bounds { get { return _bounds; } }

    void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        _bounds = transform.GetComponent<RectTransform>().GetBounds(transform.rotation);
        _contentBounds = transform.GetComponent<RectTransform>().GetBoundsWithChildren(new List<GameObject>() { gameObject }, transform.rotation);

        int dimension = (int)_scrollAlong;

        if (contentBounds.size[dimension] > bounds.size[dimension])
        {
            _minScroll = -(contentBounds.max[dimension] - bounds.max[dimension]);
            _maxScroll = -(contentBounds.min[dimension] - bounds.min[dimension]);
        }
        else
        {
            _minScroll = -(contentBounds.min[dimension] - bounds.min[dimension]);
            _maxScroll = -(contentBounds.max[dimension] - bounds.max[dimension]);
        }
        
        ScrollBy(0f);
    }

    public void Apply() { }

    public void ScrollBy(float deltaScrollInput)
    {
        float deltaScroll = Math.Clamp(deltaScrollInput * _scrollSpeed, _minScroll, _maxScroll);
        _minScroll -= deltaScroll;
        _maxScroll -= deltaScroll;

        int dimension = (int)_scrollAlong;

        foreach (Transform child in transform)
        {
            child.localPosition = transform.localPosition.ChangeValue(dimension, child.localPosition[dimension] + deltaScroll);
        }
    }
}