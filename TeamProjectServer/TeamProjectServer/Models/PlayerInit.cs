using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TeamProjectServer.Models
{
    public class PlayerInit : BaseData
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public Tier Tier { get; set; }
        public float ATKPower { get; set; }
        public float MaxHP { get; set; }
        public float HPRegenPerSec { get; set; }
        public float MaxMP { get; set; }
        public float CriticalRate { get; set; }
        public float CriticalDamage { get; set; }
        public float MPRegenPerSec { get; set; }
        public float GoldMultiplier { get; set; }
        public float CurGold {  get; set; }
        public float EXPMultiplier { get; set; }
        public float ATKSpeed { get; set; }
        public float MoveSpeed { get; set; }
    }
}
