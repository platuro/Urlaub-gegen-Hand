using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UGH.Domain.Entities
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid SenderId { get; set; }

        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; }

        [Required]
        public Guid ReceiverId { get; set; }

        [ForeignKey("ReceiverId")]
        public virtual User Receiver { get; set; }

        [MaxLength(200)]
        public string Subject { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Content { get; set; }

        [Required]
        public DateTime SentAt { get; set; }

        public DateTime? ReadAt { get; set; }

        public bool IsRead { get; set; } = false;

        public int? RelatedOfferId { get; set; }

        [ForeignKey("RelatedOfferId")]
        public virtual Offer RelatedOffer { get; set; }
    }
}
