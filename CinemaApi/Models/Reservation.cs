using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaApi.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }
        public int Qty { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string Phone { get; set; }
        public DateTime ReservationTime { get; set; } = DateTime.Now;
        public int MovieId { get; set; }
        public int UserId { get; set; }
    }
}
