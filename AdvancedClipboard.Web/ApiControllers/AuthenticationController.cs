using AdvancedClipboard.Web.ApiControllers.Data;
using AdvancedClipboard.Web.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdvancedClipboard.Web.ApiControllers
{
  [Route("api/[controller]")]
  [AllowAnonymous]
  [ApiController]
  [RequireHttps]
  public class AuthenticationController : ControllerBase
  {
    private readonly SignInManager<ApplicationUser> signInManager;

    public AuthenticationController(SignInManager<ApplicationUser> signInManager)
    {
      this.signInManager = signInManager;
    }

    [HttpPost()]
    public async Task<ActionResult> Post(AuthenticationPostData data)
    {
      var result = await signInManager.PasswordSignInAsync(userName: data.Email,
                                                           password: data.Password,
                                                           isPersistent: true,
                                                           lockoutOnFailure: false);
      if (result.Succeeded)
      {
        return this.Ok();
      }

      return this.BadRequest();
    }

  }
}
