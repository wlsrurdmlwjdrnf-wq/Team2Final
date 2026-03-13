using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaResultPresenter : MonoBehaviour
{
    [SerializeField] private Transform _resultPanel;
    [SerializeField] private ItemCardView _itemCardPrefab;

    [SerializeField] private GameEventChannelSO _eventChannel;

    private List<ItemCard> _gachaResults = new List<ItemCard>();
    private List<ItemCardView> _itemCardViews = new List<ItemCardView>();
    private WaitForSeconds _waitForSeconds = new WaitForSeconds(0.1f);

    private ItemDataSO _itemDataSO;
    private SkillDataSO _skillDataSO;
    private void Start()
    {
        PoolManager.Instance.CreatePool(_itemCardPrefab, 33, null);
    }

    private void OnEnable()
    {
        _eventChannel.OnEventRaised += HandleEvent;
        _resultPanel.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        _eventChannel.OnEventRaised -= HandleEvent;
    }

    private void HandleEvent(EGameEventType type, IGameEventPayload payload) 
    {
        if (type == EGameEventType.GachaPull && payload is ItemCard card)
        {
            _gachaResults.Add(card);
        }
        else if (type == EGameEventType.GachaRequestEnd) 
        {
            StartCoroutine(ShowCardCo());
        }
    }

    private IEnumerator ShowCardCo() 
    {
        _resultPanel.gameObject.SetActive(true);
        foreach (var card in _gachaResults) 
        {
            ItemCardView cardView = PoolManager.Instance.GetFromPool(_itemCardPrefab);
            _itemCardViews.Add(cardView);
            cardView.gameObject.transform.position = _resultPanel.transform.position;
            cardView.gameObject.transform.SetParent(_resultPanel);
            cardView.gameObject.transform.localScale = Vector3.one;

            if (card.Type == EDataType.Skill)
            {
                _skillDataSO = (ItemSkillDataManager.Instance.GetSkillData(card));
                cardView.Setup(_skillDataSO);
                InventorySystem.instance.AddSkill(_skillDataSO);
            }
            else 
            { 
                _itemDataSO = (ItemSkillDataManager.Instance.GetItemData(card));
                cardView.Setup(_itemDataSO);
                InventorySystem.instance.AddItem(_itemDataSO);
            }

            yield return _waitForSeconds;
        }
        _gachaResults.Clear();
        yield return _waitForSeconds;
        foreach (var cardView in _itemCardViews) 
        {
            if (cardView.TryGetComponent(out IPoolable poolable)) poolable.ReturnPool();
        }
        _itemCardViews.Clear();
        _resultPanel.gameObject.SetActive(false);
    }
}
