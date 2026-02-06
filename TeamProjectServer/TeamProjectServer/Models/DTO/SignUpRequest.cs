namespace TeamProjectServer.Models.DTO
{
    public class SignUpRequest
    {
        public string email { get; set; }
        public string password { get; set; }
        public string nickname { get; set; }
    }
    public class SignUpResponse : BaseResponse
    {
        public string nickname { get; set; }
        public string token { get; set; }
    }
}
