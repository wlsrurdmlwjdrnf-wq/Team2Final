using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHpMp : MonoBehaviour
{
    [SerializeField] protected HpBar _hpBar;
    [SerializeField] private MpBar _mpBar;
    public BigNumber CurrentHP { get; private set; }
    public float CurrentMana { get; private set; }

    private Coroutine _recoveryCo;
    private WaitForSeconds _recoveryInterval = new WaitForSeconds(1f);
    private ObjectFlash _flash;

    private void Start()
    {
        _flash = GetComponent<ObjectFlash>();
    }
    private void OnEnable()
    {
        CurrentHP = PlayerStatManager.Instance.MaxHP;
        CurrentMana = PlayerStatManager.Instance.MaxMana;
        _recoveryCo = StartCoroutine(RecoveryCo());

        if (_hpBar != null)
            _hpBar.UpdateHP(CurrentHP, PlayerStatManager.Instance.MaxHP);
        if (_mpBar != null)
            _mpBar.UpdateMP(CurrentMana, PlayerStatManager.Instance.MaxMana);
    }
    private void OnDisable()
    {
        if (_recoveryCo != null)
        {
            _recoveryCo = null;
        }
    }
    private IEnumerator RecoveryCo()
    {
        while (true)
        {
            // HP 회복
            if (CurrentHP < PlayerStatManager.Instance.MaxHP)
            {
                BigNumber regenAmount = PlayerStatManager.Instance.HPRegenPerSec;
                CurrentHP += regenAmount;

                if (CurrentHP > PlayerStatManager.Instance.MaxHP)
                    CurrentHP = PlayerStatManager.Instance.MaxHP;
            }

            // Mana 회복
            if (CurrentMana < PlayerStatManager.Instance.MaxMana)
            {
                float regenAmount = PlayerStatManager.Instance.ManaRegenPerSec;
                CurrentMana += regenAmount;

                if (CurrentMana > PlayerStatManager.Instance.MaxMana)
                    CurrentMana = PlayerStatManager.Instance.MaxMana;

            }

            if (_hpBar != null)
                _hpBar.UpdateHP(CurrentHP, PlayerStatManager.Instance.MaxHP);
            if (_mpBar != null)
                _mpBar.UpdateMP(CurrentMana, PlayerStatManager.Instance.MaxMana);

            yield return _recoveryInterval;
        }
    }
    public void TakeDamage(BigNumber amount)
    {
        if (amount <= new BigNumber(0)) return;

        CurrentHP -= amount;

        _flash.Flash();

        if (CurrentHP <= new BigNumber(0))
        {
            CurrentHP = new BigNumber(0);
            Die();
        }

        if (_hpBar != null)
            _hpBar.UpdateHP(CurrentHP, PlayerStatManager.Instance.MaxHP);
    }
    private void Die()
    {
        StopCoroutine(_recoveryCo);
        if (gameObject.TryGetComponent<Player>(out var player))
        {
            player.ChangeState(player.DeadState);
        }
    }

    public bool UseMana(float amount)
    {
        if (CurrentMana >= amount)
        {
            CurrentMana -= amount;
            if (_mpBar != null)
                _mpBar.UpdateMP(CurrentMana, PlayerStatManager.Instance.MaxMana);
            return true;
        }
        return false;
    }
}
