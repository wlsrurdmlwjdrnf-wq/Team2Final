using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamProjectServer.Models
{
    [Table("Weapon")]
    public class Weapon : BaseData
    {
        public string Name { get; set; }
        public EDataType Type { get; set; }
        public ElementType Element {  get; set; }
        public GradeType Grade { get; set; }
        public int Level { get; set; }
        public float EquipATK { get; set; }
        public float PassiveATK { get; set; }
        public float CriticalDMG { get; set; }
        public float CriticalRate { get; set; }
        public float GoldPer {  get; set; }
        public string IconKey { get; set; }
        public string SoundKey { get; set; }
        public string EffectKey { get; set; }
    }
}
