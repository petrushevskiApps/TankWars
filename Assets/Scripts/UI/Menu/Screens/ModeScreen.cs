using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeScreen : UIScreen
{
    protected GameModeTypes gameMode;

    [SerializeField] private TMPro.TextMeshProUGUI title;

    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardsParent;

    [SerializeField] private Button backButton;

    private List<MatchConfiguration> matchConfigurations;
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
            title.text = gameMode.ToString() + " Mode";
            matchConfigurations = GameManager.Instance.GetMatchConfigurations(gameMode);
            CreateCards();
        }
    }

    private void CreateCards()
    {
        foreach (MatchConfiguration config in matchConfigurations)
        {
            GameObject card = Instantiate(cardPrefab, cardsParent);
            card.GetComponent<UIMatchCard>().Setup(config);
        }
        isCardsGroupSet = true;
    }
}
