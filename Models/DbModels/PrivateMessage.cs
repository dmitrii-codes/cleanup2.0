using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cleanup.Models
{
    public class PrivateMessage : BaseEntity
    {
        [Key]
        public int PrivateMessageId { get; set; }
        public string Content { get; set; }
        [ForeignKey("Sender")]
        public int SenderId { get; set; }
        public User Sender { get; set; }
        [ForeignKey("Recipient")]
        public int RecipientId { get; set; }
        public User Recipient { get; set; }
        public int ReadStatus { get; set; }
    }
}



