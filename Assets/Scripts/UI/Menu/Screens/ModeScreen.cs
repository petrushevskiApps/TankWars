using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeScreen : UIScreen
{
    [SerializeField] private GameMode gameMode;

    [SerializeField] private TMPro.TextMeshProUGUI title;

    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardsParent;

    
    private void Awake()
    {
        SetTitle();
        CreateMatchCards();
    }

    private void SetTitle()
    {
        title.text = gameMode.name.ToString().Replace("Mode", "") + " Mode";
    }

    private void CreateMatchCards()
    {
        foreach (MatchConfiguration config in gameMode.matchConfigs)
        {
            GameObject card = Instantiate(cardPrefab, cardsParent);
            card.GetComponent<UIMatchCard>().Setup(config);
        }
    }
}
