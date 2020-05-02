using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

[System.Serializable]
public class CommunicationSystem
{
    [SerializeField] private TextMeshProUGUI message;

    public void UpdateMessage(string text)
    {
        message.text = text;
    }
}
