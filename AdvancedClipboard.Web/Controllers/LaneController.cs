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
      if (string.IsNullOrEmpty(model.Name))
      {
        throw new Exception("Text must not be empty.");
      }

      var userId = this.User.GetId();

      var data = TypeAdapter.Adapt<LanePostData>(model);
      await this.repository.PostAsync(userId, data);

      return LocalRedirect(model.ReturnUrl);
    }

    [HttpPost(nameof(Put))]
    public async Task<IActionResult> Put(LaneDetailsModel model)
    {
      if (string.IsNullOrEmpty(model.Name))
      {
        throw new Exception("Text must not be empty.");
      }

      var userId = this.User.GetId();

      var data = TypeAdapter.Adapt<LanePutData>(model);
      await this.repository.PutAsync(userId, data);

      return LocalRedirect(model.ReturnUrl);
    }

    [HttpPost(nameof(Delete))]
    public async Task<IActionResult> Delete(Guid id)
    {
      var userId = this.User.GetId();

      await this.repository.DeleteAsync(userId, id);

      return LocalRedirect("/");
    }

    [HttpGet(nameof(Edit))]
    public async Task<IActionResult> Edit(Guid id, string returnUrl)
    {
      var userId = this.User.GetId();

      var lanes = await repository.GetLanesForUser(userId, id);
      var data = lanes.Single();
      var model = TypeAdapter.Adapt<LaneDetailsModel>(data);
      model.ReturnUrl = returnUrl;

      return this.View(model);
    }
  }
}
