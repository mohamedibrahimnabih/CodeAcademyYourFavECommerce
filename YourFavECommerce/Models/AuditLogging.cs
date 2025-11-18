using System.ComponentModel.DataAnnotations.Schema;

namespace YourFavECommerce.Models
{
    public class AuditLogging
    {
        //public int Id { get; set; }
        //[Column(Order = 2)]
        public DateTime? CreateAT { get; set; } = DateTime.Now;
        //[Column(Order = 3)]
        public DateTime? UpdatedAT { get; set; }
    }
}
