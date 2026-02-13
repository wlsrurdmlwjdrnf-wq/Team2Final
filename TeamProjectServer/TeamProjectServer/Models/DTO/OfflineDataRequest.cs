namespace TeamProjectServer.Models.DTO
{
    public class OfflineDataRequest { }

    public class OfflineDataResponse : BaseResponse
    {
        public float time {  get; set; } // 오프라인 시간
        public float gold { get; set; } // 획득골드
        public float exp { get; set; } // 획득경험치
        public float resultGold { get; set; } // 유저 총 골드
    }
}
