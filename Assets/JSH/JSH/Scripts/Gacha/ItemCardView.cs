using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ItemCardView : MonoBehaviour, IPoolable
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _itemName;
    [SerializeField] private TextMeshProUGUI _gradeText;
    [SerializeField] private TextMeshProUGUI _tierText;

    private IPool _pool;

    public void Setup(ItemDataSO item) 
    {
        SetColor(item.Grade);
        _itemName.text = item.Name;
        _gradeText.text = item.Grade.ToString();
        _tierText.text = $"T{item.Tier.ToString()}";
    }

    public void Setup(SkillDataSO skill) 
    {
        SetColor(skill.Grade);
        _itemName.text = skill.Name;
        _gradeText.text = skill.Grade.ToString();
        _tierText.text = " ";
    }

    private void SetColor(GradeType grade) 
    {
        switch (grade)
        {
            case GradeType.Normal: _icon.color = Color.white; break;
            case GradeType.Advanced: _icon.color = Color.green; break;
            case GradeType.Rare: _icon.color = Color.blue; break;
            case GradeType.Heroic: _icon.color = new Color(1f, 0f, 1f); break;
            case GradeType.Legendary: _icon.color = Color.yellow; break;
            case GradeType.Mythical: _icon.color = Color.red; break;
        }
    }

    public void SetPool(IPool pool) { _pool = pool; }
    public void ReturnPool() 
    {
        gameObject.transform.SetParent(null);
        gameObject.transform.position = PoolManager.Instance.transform.position;
        _pool.Enqueue(this); 
    }
}
