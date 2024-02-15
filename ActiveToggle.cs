using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveToggle : MonoBehaviour, IUIElement
{
    [SerializeField] private GameObject _subject;

    public void Initialize() { }

    public void Apply() 
    { 
        _subject.SetActive(!_subject.activeSelf);
    }
}
