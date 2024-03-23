using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiChoice : MonoBehaviour, IUIElement
{
    [SerializeField] protected Layout _optionLayout;

    private int _selectedOption = -1;

    //getters and setters
    public int selectedOption { get { return _selectedOption; } }

    public void SetSelectedOption(int option = -1)
    {
        Apply();
        int newSelectedOption = option >= -1 && option < _optionLayout.transform.childCount ? option : -1;

        if (_selectedOption != newSelectedOption)
        {
            if (_selectedOption > -1)
            {
                OnDeselected(_optionLayout.transform.GetChild(_selectedOption));
            }
            if (newSelectedOption > -1)
            {
                OnSelected(_optionLayout.transform.GetChild(newSelectedOption));
            }
            _selectedOption = newSelectedOption;
        }
    }

    public void SetSelectedOption(Transform optionChildTransform)
    {
        SetSelectedOption(optionChildTransform.GetSiblingIndex());
    }

    protected virtual void OnEnable()
    {
        Apply();
    }

    public virtual void Initialize()
    {
        Apply();
    }

    public virtual void Apply() 
    {
        _selectedOption = _selectedOption >= -1 && _selectedOption < _optionLayout.transform.childCount ? _selectedOption : -1;
    }

    protected virtual void OnSelected(Transform optionChildTransform) { }
    protected virtual void OnDeselected(Transform optionChildTransform) { }
}