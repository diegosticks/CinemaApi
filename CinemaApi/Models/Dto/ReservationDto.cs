using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CinemaApi.Models.Dto
{
    public class ReservationDto
    {
        [Key]
        public int Id { get; set; }
        public int Qty { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string Phone { get; set; }
        public int MovieId { get; set; }
        public int UserId { get; set; }
    }
}
