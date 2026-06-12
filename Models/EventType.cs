

using System.ComponentModel.DataAnnotations;

namespace EventEase.Models

{
    public class EventType
    {
        [Key]
        public int EventTypeId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Event Type")]
        public string EventTypeName { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }
    }
}
