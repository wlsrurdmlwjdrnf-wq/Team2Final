using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamProjectServer.Data;
using TeamProjectServer.Models;
using TeamProjectServer.Models.DTO;
using TeamProjectServer.Services;

namespace TeamProjectServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            Console.WriteLine("가입요청");

            if (string.IsNullOrEmpty(request.email) || string.IsNullOrEmpty(request.password))
            {
                return BadRequest(new { isSuccess = false, msg = "Email 또는 Password 값을 입력해주세요"});
            }

            if (await _context.playerAccountData.AnyAsync(x => x.Email == request.email))
            {
                return BadRequest(new { isSuccess = false, msg = "중복된 이메일" });
            }

            if (await _context.playerAccountData.AnyAsync(x => x.Name == request.nickname))
            {
                return BadRequest(new { isSuccess = false, msg = "중복된 닉네임" });
            }

            var baseData = DataManager.Get<PlayerInit>(1); //임시 1번데이터

            if (baseData == null)
            {
                Console.WriteLine("데이터베이스 에러 (null)");
                return BadRequest(new { isSuccess = false, msg = "서버 데이터 로드 실패" });
            }

            var newAccount = new PlayerAccountData
            {
                Email = request.email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.password),

                Name = request.nickname,
                Level = baseData.Level,
                Tier = baseData.Tier,
                ATKPower = baseData.ATKPower,
                MaxHP = baseData.MaxHP,
                HPRegenPerSec = baseData.HPRegenPerSec,
                MaxMP = baseData.MaxMP,
                CriticalRate = baseData.CriticalRate,
                CriticalDamage = baseData.CriticalDamage,
                MPRegenPerSec = baseData.MPRegenPerSec,
                GoldMultiplier = baseData.GoldMultiplier,
                CurGold = baseData.CurGold,
                EXPMultiplier = baseData.EXPMultiplier,
                ATKSpeed = baseData.ATKSpeed,
                MoveSpeed = baseData.MoveSpeed,
                LastLoginTime = DateTime.UtcNow,
                Inventory = DataManager.GetInitInventoryData()
            };

            _context.playerAccountData.Add(newAccount);
            await _context.SaveChangesAsync();
            string newToken = _jwtService.CreateToken(newAccount.Email, newAccount.Name);

            return Ok(new { isSuccess = true, msg = "회원가입 성공", token = newToken });
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.playerAccountData.FirstOrDefaultAsync(x => x.Email == request.email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.password, user.Password))
            {
                return Unauthorized(new { isSuccess = false, msg = "이메일 또는 비밀번호를 확인해주세요." });
            }

            string newToken = _jwtService.CreateToken(user.Email, user.Name);

            user.LastLoginTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                isSuccess = true,
                token = newToken,
                msg = "로그인 성공",
                nickname = user.Name
            });
        }        
    }
}
