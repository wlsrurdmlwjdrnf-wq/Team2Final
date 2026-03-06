using UnityEngine;

public static class PlayerLevelUpSystem
{
    private static BigNumber _expRequirement = new BigNumber(100);

    // 레벨 업 버튼 누를 때 호출
    public static void TryPlayerLevelUp()
    {
        _expRequirement = new BigNumber(Mathf.Pow(PlayerStatManager.Instance.PlayerLevel * 10, 2));

        if (PlayerResourceManager.Instance.GetResource(ResourceType.EXP) < _expRequirement) return;

        SoundManager.Instance.PlaySFX(ESFXType.LevelUpButton);
        PlayerStatManager.Instance.LevelUp();
        PlayerResourceManager.Instance.AddResource(ResourceType.StatPoint, new BigNumber(5));
        PlayerResourceManager.Instance.SpendResource(ResourceType.EXP, _expRequirement);
    }

    // 경험치 필요량 UI로 보여주기용
    public static string GetExpRequirement()
    {
        _expRequirement = new BigNumber(Mathf.Pow(PlayerStatManager.Instance.PlayerLevel * 10, 2));
        return BigNumberFormatter.ToFormatted(_expRequirement);
    }

    // 경험치 % 보여주기
    public static string GetExpRate()
    {
        BigNumber bn = PlayerResourceManager.Instance.GetResource(ResourceType.EXP) / _expRequirement * new BigNumber(100);
        if (bn > new BigNumber(100)) bn = new BigNumber(100);
        return BigNumberFormatter.ToFormatted(bn) + "%";
    }
}
