using Microsoft.AspNetCore.Mvc;
using TeamProjectServer.Models;
using TeamProjectServer.Services;

namespace TeamProjectServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InitDataController : ControllerBase
    {
        [HttpPost("sync")]
        public async Task<IActionResult> SyncSheets([FromQuery] string url)
        {
            if (string.IsNullOrEmpty(url)) return BadRequest();
            await DataManager.SyncSheet(url);
            return Ok("데이터동기화완료");
        }
        [HttpGet("check-data-playerInit")]
        public IActionResult CheckPlayerInit(int id)
        {
            return Ok(DataManager.Get<PlayerInit>(id));
        }
        [HttpGet("check-data-weapon")]
        public IActionResult CheckWeapon(int id)
        {
            return Ok(DataManager.Get<Weapon>(id));
        }
        [HttpGet("check-data-Accessory")]
        public IActionResult CheckAccessory(int id)
        {
            return Ok(DataManager.Get<Accessory>(id));
        }
        [HttpGet("check-data-Artifact")]
        public IActionResult CheckArtifact(int id)
        {
            return Ok(DataManager.Get<Artifact>(id));
        }
        [HttpGet("check-data-Skill")]
        public IActionResult CheckSkill(int id)
        {
            return Ok(DataManager.Get<Skill>(id));
        }
        [HttpGet("check-data-Stage")]
        public IActionResult CheckStage(int id)
        {
            return Ok(DataManager.Get<Stage>(id));
        }
    }
}
