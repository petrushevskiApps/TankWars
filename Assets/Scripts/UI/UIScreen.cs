using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreen : MonoBehaviour, IEquatable<UIScreen>
{
    protected void OnEnable()
    {
        InputController.OnBackKey.AddListener(OnBackPressed);
    }
    protected void OnDisable()
    {
        InputController.OnBackKey.RemoveListener(OnBackPressed);
    }
    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnBackPressed()
    {
        UIController.Instance.OnBack();
    }

    public bool Equals(UIScreen other)
    {
        return gameObject.name.Equals(other.gameObject.name);
    }
}
