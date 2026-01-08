using System.Security.Claims;

namespace UGHApi.Services.UserProvider;

public class UserProvider : IUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public Guid UserId { get; private set; } = Guid.Empty;
    public bool IsAuthenticated { get; private set; } = false;

    public UserProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;

        var user = _httpContextAccessor?.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                UserId = userId;
                IsAuthenticated = true;
            }
        }
    }
}
