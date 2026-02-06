using System.ComponentModel.DataAnnotations;

namespace TeamProjectServer.Models
{
    public abstract class BaseData
    {
        [Key]
        public int ID { get; set; }
    }
}
