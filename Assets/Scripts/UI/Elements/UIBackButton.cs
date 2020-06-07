using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBackButton : MonoBehaviour
{
    private Button backButton;

    protected void Awake()
    {
        backButton = GetComponent<Button>();
        backButton.onClick.AddListener(OnBackButton);
    }
    private void OnDestroy()
    {
        backButton.onClick.RemoveListener(OnBackButton);
    }

    private void OnBackButton()
    {
        UIController.Instance.OnBack();
    }
}
