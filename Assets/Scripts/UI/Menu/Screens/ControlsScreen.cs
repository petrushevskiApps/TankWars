using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsScreen : UIScreen
{
    [SerializeField] private Button backButton;

    protected void Awake()
    {
        backButton.onClick.AddListener(UIController.Instance.OnBack);
    }
    private void OnDestroy()
    {
        backButton.onClick.RemoveListener(UIController.Instance.OnBack);
    }
}
