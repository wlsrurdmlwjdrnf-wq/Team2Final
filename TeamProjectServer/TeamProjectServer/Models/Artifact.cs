using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamProjectServer.Models
{
    public class Artifact : BaseData
    {
        public string Name { get; set; }
        public EDataType Type { get; set; }
        public ElementType Element {  get; set; }
        public GradeType Grade { get; set; }
        public int Level { get; set; }
        public string Icon { get; set; }
    }
}
