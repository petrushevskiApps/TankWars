using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDScreen : UIScreen
{
    public override void OnBackButton()
    {
        GameManager.Instance.PauseGame();
        UIController.Instance.ShowScreen<PauseScreen>();
    }
}
