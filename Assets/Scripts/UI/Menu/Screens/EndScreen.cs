using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreen : UIScreen
{
    public override void OnBackButton()
    {
        UIController.Instance.ShowScreen<StartScreen>();
    }
}
