using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHpMp : MonoBehaviour
{
    //임시 체력
    [SerializeField] private TextMeshProUGUI hphp;
    public BigNumber CurrentHP { get; private set; }
    public float CurrentMana { get; private set; }

    private Coroutine _recoveryCo;
    private WaitForSeconds _recoveryInterval = new WaitForSeconds(1f);

    private void OnEnable()
    {
        CurrentHP = PlayerStatManager.Instance.MaxHP;
        CurrentMana = PlayerStatManager.Instance.MaxMana;
        _recoveryCo = StartCoroutine(RecoveryCo());
        hphp.text = BigNumberFormatter.ToFormatted(CurrentHP);
    }
    private void OnDisable()
    {
        if (_recoveryCo != null)
        {
            StopCoroutine(_recoveryCo);
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

            yield return _recoveryInterval;
        }
    }
    public void TakeDamage(BigNumber amount)
    {
        if (amount <= new BigNumber(0)) return;

        CurrentHP -= amount;
        hphp.text = BigNumberFormatter.ToFormatted(CurrentHP); // 테스트
        if (CurrentHP <= new BigNumber(0))
        {
            Die();
        }
    }
    private void Die()
    {
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
            return true;
        }
        return false;
    }
}
