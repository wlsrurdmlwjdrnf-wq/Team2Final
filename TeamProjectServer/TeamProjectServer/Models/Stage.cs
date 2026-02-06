using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamProjectServer.Models
{
    public class Stage : BaseData
    {
        public string Name { get; set; }
        public ElementType Element {  get; set; }
        public GradeType Grade { get; set; }
        public int Level { get; set; }
        public bool isClear { get; set; }
    }
}
