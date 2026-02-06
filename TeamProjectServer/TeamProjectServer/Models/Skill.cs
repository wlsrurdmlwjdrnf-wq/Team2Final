using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamProjectServer.Models
{
    public class Skill : BaseData
    {
        public string Name { get; set; }
        public EDataType Type { get; set; }
        public ElementType Elemnet {  get; set; }
        public GradeType Grade { get; set; }
        public int Level { get; set; }
        public string Icon { get; set; }
        public string Sound { get; set; }
        public string Effect { get; set; }
    }
}
