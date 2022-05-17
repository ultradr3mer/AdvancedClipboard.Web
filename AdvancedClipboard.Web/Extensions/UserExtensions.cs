using AdvancedClipboard.Web.Models.Identity;
using System.Security.Claims;

namespace AdvancedClipboard.Web.Extensions
{
    public static class UserExtensions
    {
        public static Guid GetId(this ClaimsPrincipal user)
        {
            return Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}
