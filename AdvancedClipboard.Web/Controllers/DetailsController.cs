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
  public class DetailsController : Controller
  {
    private readonly ClipboardRepository repository;

    public DetailsController(ClipboardRepository repository)
    {
      this.repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(Guid id, string returnurl)
    {
      var userId = this.User.GetId();

      var data = await this.repository.GetWithContextAsync(id, userId);

      var model = TypeAdapter.Adapt<DetailsModel>(data.Entries.Single());
      model.Lanes = new SelectList(new[] { new LaneGetData() }.Concat(data.Lanes), "Id", "Name");
      model.ReturnUrl = returnurl;

      return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(DetailsModel model)
    {
      var userId = this.User.GetId();

      var data = TypeAdapter.Adapt<ClipboardPutData>(model);
      await this.repository.Put(data, userId);

      return LocalRedirect(model.ReturnUrl);
    }

    [HttpPost(nameof(Delete))]
    public async Task<IActionResult> Delete(DetailsModel model)
    {
      var userId = this.User.GetId();

      await this.repository.Delete(model.Id, userId);

      return LocalRedirect(model.ReturnUrl);
    }
  }
}
