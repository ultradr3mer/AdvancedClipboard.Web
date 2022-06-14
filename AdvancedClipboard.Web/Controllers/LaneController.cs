using AdvancedClipboard.Web.ApiControllers.Data;
using AdvancedClipboard.Web.Controllers.Model;
using AdvancedClipboard.Web.Extensions;
using AdvancedClipboard.Web.Repositories;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdvancedClipboard.Web.Controllers
{
  [Authorize]
  [Route("[controller]")]
  public class LaneController : Controller
  {
    private readonly LaneRepository repository;

    public LaneController(LaneRepository repository)
    {
      this.repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> Post(LaneDetailsModel model)
    {
      var userId = this.User.GetId();

      var data = TypeAdapter.Adapt<LanePostData>(model);
      await this.repository.PostAsync(data, userId);

      return LocalRedirect(model.ReturnUrl);
    }
  }
}
