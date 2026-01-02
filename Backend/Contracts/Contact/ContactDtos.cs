using System;
using System.Collections.Generic;

namespace Backend.Contracts.Contact
{
    public class SendMessageDto
    {
        public Guid ReceiverId { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public int? RelatedOfferId { get; set; }
    }

    public class MessageDto
    {
        public int Id { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderAvatar { get; set; }
        public Guid ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverAvatar { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsRead { get; set; }
        public int? RelatedOfferId { get; set; }
    }

    public class MessagesResponseDto
    {
        public List<MessageDto> Messages { get; set; }
        public int TotalCount { get; set; }
        public int PageCount { get; set; }
    }

    public class UnreadCountDto
    {
        public int Count { get; set; }
    }
}
