using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInfoView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerLevelText;
    [SerializeField] private TextMeshProUGUI playerAtkText;
    [SerializeField] private TextMeshProUGUI playerHpText;
    [SerializeField] private TextMeshProUGUI playerHpRegenText;
    [SerializeField] private TextMeshProUGUI playeCriRateText;
    [SerializeField] private TextMeshProUGUI playerCriDmgText;
    [SerializeField] private TextMeshProUGUI playerManaText;
    [SerializeField] private TextMeshProUGUI playerManaRegenText;
    [SerializeField] private TextMeshProUGUI playerGoldMultiText;
    [SerializeField] private TextMeshProUGUI playerExpMultiText;

    private PlayerStatManager _psm;
    private void Start()
    {
        _psm = PlayerStatManager.Instance;
        _psm.OnStatChanged += UpdateInfo;
    }
    private void UpdateInfo()
    {
        playerLevelText.text = "Lv. "+_psm.PlayerLevel.ToString();
        playerAtkText.text = _psm.GetFormatted(StatType.AttackPower);
        playerHpText.text = _psm.GetFormatted(StatType.MaxHP);
        playerHpRegenText.text = _psm.GetFormatted(StatType.HPRegenPerSec);
        playeCriRateText.text = BigNumberFormatter.ToFormatted(_psm.CritRate * 100f)+ "%";
        playerCriDmgText.text = BigNumberFormatter.ToFormatted(_psm.CritDamage * new BigNumber(100)) + "%";
        playerManaText.text = _psm.GetFormatted(StatType.MaxMana);
        playerManaRegenText.text = _psm.GetFormatted(StatType.ManaRegenPerSec);
        playerGoldMultiText.text = BigNumberFormatter.ToFormatted(_psm.GoldMultiplier * new BigNumber(100) - new BigNumber(100))+ "%";
        playerExpMultiText.text = BigNumberFormatter.ToFormatted(_psm.ExpMultiplier * new BigNumber(100) - new BigNumber(100)) + "%";
    }
    private void OnDestroy()
    {
        if (PlayerStatManager.Instance == null) return;
        PlayerStatManager.Instance.OnStatChanged -= UpdateInfo;
    }
}
