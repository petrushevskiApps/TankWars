using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreen : MonoBehaviour, IEquatable<UIScreen>
{
    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public bool Equals(UIScreen other)
    {
        return gameObject.name.Equals(other.gameObject.name);
    }
}
