
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLDV_Part1.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        // ✅ Explicit FK
        [ForeignKey("Event")]
        public int EventId { get; set; }

        // ✅ Explicit FK
        [ForeignKey("Venue")]
        public int VenueId { get; set; }

        [Required]
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; }

        // Navigation properties
        public Event Event { get; set; }
        public Venue Venue { get; set; }
    }
}