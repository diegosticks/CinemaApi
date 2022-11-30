using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CinemaApi.Models.Dto
{
    public class MovieDto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name cannot be null or empty.")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Language cannot be null or empty.")]
        public string Language { get; set; }

        public string Duration { get; set; }
        public DateTime PlayingDate { get; set; }
        public DateTime PlayingTime { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TicketPrice { get; set; }

        [Required]
        public double Rating { get; set; }
        public string Genre { get; set; }
        public string TrailUrl { get; set; }

        public string? ImageUrl { get; set; }

        [NotMapped]
        public IFormFile Image { get; set; }
    }
}
