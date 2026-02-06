namespace TeamProjectServer.Models.DTO
{
    public class LoginRequest
    {
        public string email { get; set; }
        public string password { get; set; }
    }
    public class LoginResponse : BaseResponse
    {
        public string nickname { get; set; }
        public string token { get; set; }
        //public PlayerAccountData playerData { get; set; } // 해당 플레이어의 데이터
        //public float offlineRewardGold { get; set; } // 오프라인 보상 골드
        //public float offlineRewardEXP { get; set; } // 오프라인 보상 경험치
        //public float offlineMinutes { get; set; } // 부재시간
    }
}
