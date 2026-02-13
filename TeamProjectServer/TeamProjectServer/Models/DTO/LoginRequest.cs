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

    }
}
