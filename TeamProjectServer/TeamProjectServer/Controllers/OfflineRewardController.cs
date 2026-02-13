using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamProjectServer.Data;
using System.Security.Claims;

namespace TeamProjectServer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OfflineRewardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OfflineRewardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("offline")]
        public async Task<IActionResult> OfflineReward()
        {
            var userIDClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIDClaim, out int userID))
            {
                return Unauthorized(new { isSuccess = false, msg = "인증토큰 오류" });
            }

            var user = await _context.playerAccountData.FindAsync(userID);

            if (user == null)
            {
                return BadRequest(new { isSuccess = false, msg = "유저를 찾을 수 없습니다" });
            }

            //인증성공

            DateTime now = DateTime.UtcNow;
            TimeSpan offlineTime = now - user.LastLoginTime;
            double minutes = offlineTime.TotalMinutes;

            if (minutes < 1)
            {
                return Ok(new { isSuccess = false, msg = "1분 미만은 오프라인 획득 불가" });
            }

            //최대 24시간 보상획득
            if (minutes > 1440) minutes = 1440;


            //골드획득 로직 ex) 오프라인시간 x 플레이어 골드획득량 x 스테이지 골드(총 획득골드 / 분)
            
            // user.CurGold = 계산된 총 골드 
            // exp = 계산된 총 경험치

            //시간 갱신
            user.LastLoginTime = now;
            
            // DB 유저 데이터 갱신
            await _context.SaveChangesAsync();


            return Ok(new
            {
                isSuccess = true,
                msg = $"오프라인 보상 획득",
                time = (float)Math.Round(minutes, 1),
                // gold = 계산된 값
                //exp = 계산된 값
                resultGold = user.CurGold
            });
        }
    }
}
