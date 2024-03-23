using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiChoice : MonoBehaviour, IUIElement
{
    [SerializeField] private Layout _optionLayout;

    private int _selectedOption = -1;
    public int selectedOption { get { return Math.Clamp(_selectedOption, -1, _optionLayout.transform.childCount - 1); } set { _selectedOption = value; } }

    public void SetSelectedOption(int option = -1)
    { 
        int newSelectedOption = Math.Clamp(option, -1, _optionLayout.transform.childCount - 1);
        if (_selectedOption != newSelectedOption)
        {
            OnDeselected(_optionLayout.transform.GetChild(_selectedOption));
            OnSelected(_optionLayout.transform.GetChild(newSelectedOption));
            _selectedOption = newSelectedOption;
        }
    }

    public void SetSelectedOption(Transform optionChildTransform)
    {
        SetSelectedOption(optionChildTransform.GetSiblingIndex());
    }

    void OnEnable()
    {
        Initialize();
    }

    public virtual void Initialize()
    {
        _optionLayout.Apply();
    }

    public virtual void Apply() { }

    protected virtual void OnSelected(Transform optionChildTransform) { }
    protected virtual void OnDeselected(Transform optionChildTransform) { }
}