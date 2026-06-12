using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EventEase.Models
{
    public class Venue
    {
       
            [Key]
            public int VenueId { get; set; }

            [Required]
            [StringLength(100)]
            public string VenueName { get; set; }

            [Required]
            [StringLength(150)]
            public string Location { get; set; }

            [Required]
        
            [Display(Name = "Available")]
            
        public bool IsAvailable { get; set; } = true;
        public int Capacity { get; set; }

            [Display(Name = "Venue Image")]
            public string? ImageUrl { get; set; }

        // Navigation property
        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<Event>? Events { get; set; }
    }
}

