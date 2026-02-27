using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrowUpView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerLv;
    [SerializeField] private TextMeshProUGUI expRequirementText;
    [SerializeField] private Button levelUpButton;

    [SerializeField] private TextMeshProUGUI strText;
    [SerializeField] private TextMeshProUGUI strLvText;
    [SerializeField] private Button strUpButton;

    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI hpLvText;
    [SerializeField] private Button hpUpButton;

    [SerializeField] private TextMeshProUGUI vitText;
    [SerializeField] private TextMeshProUGUI vitLvText;
    [SerializeField] private Button vitUpButton;

    [SerializeField] private TextMeshProUGUI criText;
    [SerializeField] private TextMeshProUGUI criLvText;
    [SerializeField] private Button criUpButton;

    [SerializeField] private TextMeshProUGUI lukText;
    [SerializeField] private TextMeshProUGUI lukLvText;
    [SerializeField] private Button lukUpButton;

    public event Action OnLevelUpButtonClicked;
    public event Action OnStrUpButtonClicked;
    public event Action OnHpUpButtonClicked;
    public event Action OnVitUpButtonClicked;
    public event Action OnCriUpButtonClicked;
    public event Action OnLukUpButtonClicked;

    private void Awake()
    {
        levelUpButton.onClick.AddListener(() => OnLevelUpButtonClicked?.Invoke());
        strUpButton.onClick.AddListener(() => OnStrUpButtonClicked?.Invoke());
        hpUpButton.onClick.AddListener(() => OnHpUpButtonClicked?.Invoke());
        vitUpButton.onClick.AddListener(() => OnVitUpButtonClicked?.Invoke());
        criUpButton.onClick.AddListener(() => OnCriUpButtonClicked?.Invoke());
        lukUpButton.onClick.AddListener(() => OnLukUpButtonClicked?.Invoke());
    }
    public void UpdateLevel(string level, string expRequirement)
    {
        playerLv.text = "Lv." + level;
        expRequirementText.text = "/\t" + expRequirement;
    }
    public void UpdateStr(string str, string strLv)
    {
        strText.text = str;
        strLvText.text = "Lv." + strLv;
    }
    public void UpdateHp(string hp, string hpLv)
    {
        hpText.text = hp;
        hpLvText.text = "Lv." + hpLv;
    }
    public void UpdateVit(string vit, string vitLv)
    {
        vitText.text = vit;
        vitLvText.text = "Lv." + vitLv;
    }
    public void UpdateCri(string criDmg, string criDmgLv)
    {
        criText.text = criDmg;
        criLvText.text = "Lv." + criDmgLv;
    }
    public void UpdateLuk(string luk, string lukLv)
    {
        lukText.text = luk;
        lukLvText.text = "Lv." + lukLv;
    }
}
