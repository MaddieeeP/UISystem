using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Layout))]

public class MultiChoice : MonoBehaviour, IUIElement
{
    [SerializeField] private GameObject optionPrefab;
    private int _optionCount;
    public int optionCount 
    { 
        get 
        {
            return _optionCount; 
        }
        
        set
        {
            _optionCount = value;
            Initialize();
        }
    }
    private int _selectedOption = -1;
    public int selectedOption { get { return Math.Clamp(_selectedOption, -1, _optionCount - 1); } set { _selectedOption = value; } }

    void OnEnable()
    {
        Initialize();
    }

    public void Initialize(int newOptionCount)
    {
        _optionCount = newOptionCount;
        Initialize();
    }

    public void Initialize()
    {
        if (transform.childCount > 0)
        {
            gameObject.DestroyAllChildren();
        }
        if (optionPrefab == null || _optionCount < 1)
        {
            return;
        }
        for (int i = 0; i < _optionCount; i++)
        {
            Instantiate(optionPrefab, transform);
        }

        transform.GetComponent<Layout>().Apply();
    }

    public void Apply() { }

    void SetValue(Transform child)
    {
        _selectedOption = child.GetSiblingIndex();
    }
}