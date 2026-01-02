using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Backend.Contracts.Contact;
using UGHApi.DATA;
using UGH.Domain.Entities;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContactController : ControllerBase
    {
        private readonly Ugh_Context _context;
        private readonly ILogger<ContactController> _logger;

        public ContactController(Ugh_Context context, ILogger<ContactController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            try
            {
                var senderIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(senderIdClaim) || !Guid.TryParse(senderIdClaim, out var senderId))
                {
                    return Unauthorized("Ungültige Benutzer-ID");
                }

                var sender = await _context.users
                    .FirstOrDefaultAsync(u => u.User_Id == senderId);

                var receiver = await _context.users
                    .FirstOrDefaultAsync(u => u.User_Id == dto.ReceiverId);

                if (sender == null || receiver == null)
                {
                    return NotFound("Benutzer nicht gefunden");
                }

                var hasActiveMembership = await _context.Set<UserMembership>().AnyAsync(um => um.User_Id == senderId && um.Expiration > DateTime.UtcNow);

                if (!hasActiveMembership)
                {
                    return StatusCode(403, new 
                    { 
                        message = "Sie benötigen eine aktive Mitgliedschaft um Nachrichten zu senden",
                        requiresMembership = true 
                    });
                }

                if (senderId == dto.ReceiverId)
                {
                    return BadRequest("Sie können keine Nachricht an sich selbst senden");
                }

                if (string.IsNullOrWhiteSpace(dto.Content))
                {
                    return BadRequest("Nachricht darf nicht leer sein");
                }

                if (dto.Content.Length > 2000)
                {
                    return BadRequest("Nachricht darf maximal 2000 Zeichen lang sein");
                }

                var message = new Message
                {
                    SenderId = senderId,
                    ReceiverId = dto.ReceiverId,
                    Subject = dto.Subject ?? "Neue Nachricht",
                    Content = dto.Content,
                    SentAt = DateTime.UtcNow,
                    IsRead = false,
                    RelatedOfferId = dto.RelatedOfferId
                };

                _context.messages.Add(message);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Nachricht {message.Id} von {senderId} an {dto.ReceiverId} gesendet");

                return Ok(new { messageId = message.Id, message = "Nachricht erfolgreich gesendet" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Senden der Nachricht");
                return StatusCode(500, "Interner Serverfehler");
            }
        }

        [HttpGet("inbox")]
        public async Task<IActionResult> GetInbox([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized();
                }

                var totalCount = await _context.messages.CountAsync(m => m.ReceiverId == userId);

                var messages = await _context.messages
                    .Where(m => m.ReceiverId == userId)
                    .Include(m => m.Sender)
                    .OrderByDescending(m => m.SentAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(m => new MessageDto
                    {
                        Id = m.Id,
                        SenderId = m.SenderId,
                        SenderName = m.Sender.FirstName + " " + m.Sender.LastName,
                        SenderAvatar = m.Sender.ProfilePicture != null ? Convert.ToBase64String(m.Sender.ProfilePicture) : null,
                        ReceiverId = m.ReceiverId,
                        Subject = m.Subject,
                        Content = m.Content,
                        SentAt = m.SentAt,
                        ReadAt = m.ReadAt,
                        IsRead = m.IsRead,
                        RelatedOfferId = m.RelatedOfferId
                    })
                    .ToListAsync();

                return Ok(new MessagesResponseDto
                {
                    Messages = messages,
                    TotalCount = totalCount,
                    PageCount = (int)Math.Ceiling(totalCount / (double)pageSize)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Abrufen des Posteingangs");
                return StatusCode(500, "Interner Serverfehler");
            }
        }

        [HttpGet("sent")]
        public async Task<IActionResult> GetSent([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized();
                }

                var totalCount = await _context.messages.CountAsync(m => m.SenderId == userId);

                var messages = await _context.messages
                    .Where(m => m.SenderId == userId)
                    .Include(m => m.Receiver)
                    .OrderByDescending(m => m.SentAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(m => new MessageDto
                    {
                        Id = m.Id,
                        SenderId = m.SenderId,
                        ReceiverId = m.ReceiverId,
                        ReceiverName = m.Receiver.FirstName + " " + m.Receiver.LastName,
                        ReceiverAvatar = m.Receiver.ProfilePicture != null ? Convert.ToBase64String(m.Receiver.ProfilePicture) : null,
                        Subject = m.Subject,
                        Content = m.Content,
                        SentAt = m.SentAt,
                        ReadAt = m.ReadAt,
                        IsRead = m.IsRead,
                        RelatedOfferId = m.RelatedOfferId
                    })
                    .ToListAsync();

                return Ok(new MessagesResponseDto
                {
                    Messages = messages,
                    TotalCount = totalCount,
                    PageCount = (int)Math.Ceiling(totalCount / (double)pageSize)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Abrufen gesendeter Nachrichten");
                return StatusCode(500, "Interner Serverfehler");
            }
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized();
                }

                var count = await _context.messages.CountAsync(m => m.ReceiverId == userId && !m.IsRead);

                return Ok(new UnreadCountDto { Count = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Zählen ungelesener Nachrichten");
                return StatusCode(500, "Interner Serverfehler");
            }
        }

        [HttpPut("{id}/mark-read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized();
                }

                var message = await _context.messages.FindAsync(id);

                if (message == null)
                {
                    return NotFound("Nachricht nicht gefunden");
                }

                if (message.ReceiverId != userId)
                {
                    return Forbid();
                }

                if (!message.IsRead)
                {
                    message.IsRead = true;
                    message.ReadAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "Nachricht als gelesen markiert" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Markieren der Nachricht");
                return StatusCode(500, "Interner Serverfehler");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized();
                }

                var message = await _context.messages.FindAsync(id);

                if (message == null)
                {
                    return NotFound("Nachricht nicht gefunden");
                }

                if (message.SenderId != userId && message.ReceiverId != userId)
                {
                    return Forbid();
                }

                _context.messages.Remove(message);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Nachricht gelöscht" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Löschen der Nachricht");
                return StatusCode(500, "Interner Serverfehler");
            }
        }
    }
}
