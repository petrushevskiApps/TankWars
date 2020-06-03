using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeScreen : UIScreen
{
    protected GameModeTypes gameModeType;
    [SerializeField] private GameMode gameMode;

    [SerializeField] private TMPro.TextMeshProUGUI title;

    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardsParent;

    [SerializeField] private Button backButton;

    
    private bool isCardsGroupSet = false;

    protected void Awake()
    {
        backButton.onClick.AddListener(UIController.Instance.OnBack);
    }
    private void OnDestroy()
    {
        backButton.onClick.RemoveListener(UIController.Instance.OnBack);
    }
    private new void OnEnable()
    {
        base.OnEnable();

        if (!isCardsGroupSet)
        {
            title.text = gameModeType.ToString() + " Mode";
            CreateMatchCards();
        }
    }

    private void CreateMatchCards()
    {
        foreach (MatchConfiguration config in gameMode.matchConfigs)
        {
            GameObject card = Instantiate(cardPrefab, cardsParent);
            card.GetComponent<UIMatchCard>().Setup(config);
        }
        isCardsGroupSet = true;
    }
}
