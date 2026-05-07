using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models

{
  [Keyless]
        public class BookingDetailsView
        {
            public int BookingId { get; set; }

            [Display(Name = "Booking Date")]
            public DateTime BookingDate { get; set; }

            public int VenueId { get; set; }

            [Display(Name = "Venue Name")]
            public string VenueName { get; set; } = string.Empty;

            [Display(Name = "Location")]
            public string Location { get; set; } = string.Empty;

            [Display(Name = "Capacity")]
            public int Capacity { get; set; }

            public int EventId { get; set; }

            [Display(Name = "Event Name")]
            public string EventName { get; set; } = string.Empty;

            [Display(Name = "Event Date")]
            public DateTime EventDate { get; set; }

            [Display(Name = "Description")]
            public string? Description { get; set; }
        }
    }

