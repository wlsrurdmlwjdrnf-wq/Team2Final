using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamProjectServer.Models
{
    public class Accessory : BaseData
    {
        public string Name { get; set; }
        public EDataType Type { get; set; }
        public ElementType Element {  get; set; }
        public GradeType Grade { get; set; }
        public int Level { get; set; }
        public float Hp { get; set; }
        public float HPPer {  get; set; }
        public float MPPer { get; set; }
        public float GoldPer { get; set; }
        public string IconKey { get; set; }
    }
}
