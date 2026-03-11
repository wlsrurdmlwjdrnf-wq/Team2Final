using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class ItemCardView : MonoBehaviour, IPoolable
{
    [SerializeField] private Image _icon;
    [SerializeField] private Image _Image;  
    [SerializeField] private TextMeshProUGUI _itemName;
    [SerializeField] private TextMeshProUGUI _gradeText;
    [SerializeField] private TextMeshProUGUI _tierText;

    private IPool _pool;

    public void Setup(ItemDataSO item) 
    {
        SetColor(item.Grade);

        _ = LoadIcon(item.Name);
        _itemName.text = item.Name;
        _gradeText.text = item.Grade.ToString();
        _tierText.text = $"T{item.Tier.ToString()}";
        SoundManager.Instance.PlaySFX(ESFXType.ItemDrop);
    }

    public void Setup(SkillDataSO skill) 
    {
        SetColor(skill.Grade);
        _ = LoadIcon(Enum.GetName(typeof(ESkillEffectType), skill.SkillType));
        _itemName.text = skill.Name;
        _gradeText.text = skill.Grade.ToString();
        _tierText.text = " ";
        SoundManager.Instance.PlaySFX(ESFXType.ItemDrop);
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
            case GradeType.Immortal: _icon.color = Color.black; break;
        }
    }
    private async Task LoadIcon(string address)
    {
        AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(address);

        Sprite sprite = await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _Image.sprite = sprite;
        }
        else
        {
            Debug.Log($"IconLoadFailed{address}");
        }
    }
    public void SetPool(IPool pool) { _pool = pool; }
    public void ReturnPool() 
    {
        if (_pool is MonoBehaviour mono)
        gameObject.transform.SetParent(mono.transform);
        gameObject.transform.position = PoolManager.Instance.transform.position;
        _pool.Enqueue(this); 
    }
}
