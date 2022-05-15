using Microsoft.AspNetCore.Identity;

namespace AdvancedClipboard.Web.Models.Identity
{
    public class ApplicationUser : IdentityUser<Guid> { }

    public class Role : IdentityRole<Guid> { }
}

