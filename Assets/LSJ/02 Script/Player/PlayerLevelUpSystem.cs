using UnityEngine;

public static class PlayerLevelUpSystem
{
    private static BigNumber _expRequirement;

    // 레벨 업 버튼 누를 때 호출
    public static void TryPlayerLevelUp()
    {
        _expRequirement = new BigNumber(Mathf.Pow(PlayerStatManager.Instance.PlayerLevel * 10, 2));

        if (PlayerResourceManager.Instance.GetResource(ResourceType.EXP) < _expRequirement) return;

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
}
