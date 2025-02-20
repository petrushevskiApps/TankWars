﻿using System;
using UnityEngine;
using UnityEngine.Events;

public interface IDestroyable
{
    void RegisterOnDestroy(UnityAction<GameObject> OnDestroyAction);
    void UnregisterOnDestroy(UnityAction<GameObject> OnDestroyAction);
}