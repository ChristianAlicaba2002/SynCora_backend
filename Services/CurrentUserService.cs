using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using syncora_server.Interface.ICurrentUser;

namespace syncora_server.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    public Guid? UserId
    {
        get
        {
            var id = Principal?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return Guid.TryParse(id, out var userId) ? userId : null;
        }
    }

    public string? Email =>
        Principal?.FindFirstValue(JwtRegisteredClaimNames.Email)
        ?? Principal?.FindFirstValue(ClaimTypes.Email);
}
