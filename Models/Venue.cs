using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace CLDV_Part1.Models
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
           
            public int Capacity { get; set; }

            [Display(Name = "Venue Image")]
            public string? ImageUrl { get; set; }

        // Navigation property
        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<Event>? Events { get; set; }
    }
}

