using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreen : MonoBehaviour, IEquatable<UIScreen>
{
    protected void OnEnable()
    {
        InputController.OnEscapePressed.AddListener(OnBackButton);
    }
    private void OnDisable()
    {
        InputController.OnEscapePressed.RemoveListener(OnBackButton);
    }
    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnBackButton()
    {
        UIController.Instance.OnBack();
    }

    public bool Equals(UIScreen other)
    {
        return gameObject.name.Equals(other.gameObject.name);
    }
}
