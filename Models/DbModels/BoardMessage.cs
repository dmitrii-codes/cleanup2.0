using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cleanup.Models
{
    public class BoardMessage: BaseEntity
    {
        [Key]
        public int BoardMessageId { get; set; }
        [Required]
        public string Content { get; set; }
        [ForeignKey("Sender")]
        public int SenderId { get; set; }
        public User Sender { get; set; }
        [ForeignKey("Event")]
        public int EventId { get; set; }
        public CleanupEvent Event { get; set; }
    }
}