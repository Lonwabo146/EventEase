using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLDV_Part1.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        [StringLength(120)]
        public string EventName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Event Date")]
        public DateTime EventDate { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        // Nullable foreign key
        public int? VenueId { get; set; }

        // Navigation property
        [ForeignKey("VenueId")]
        public Venue? Venue { get; set; }
        // Navigation property
        public ICollection<Booking>? Bookings { get; set; }
    }
}