using AdvancedClipboard.Web.ApiControllers;
using AdvancedClipboard.Web.Controllers.Model;
using AdvancedClipboard.Web.Extensions;
using AdvancedClipboard.Web.Models;
using AdvancedClipboard.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AdvancedClipboard.Web.Controllers
{
  [Authorize]
  public class HomeController : Controller
  {
    private readonly ClipboardRepository repository;

    public HomeController(ClipboardRepository clipboardController)
    {
      this.repository = clipboardController;
    }

    public async Task<IActionResult> Index(Guid? id = null)
    {
      var userId = this.User.GetId();

      var data = await this.repository.GetWithContextAsync(id, userId);

      var model = new HomeIndexModel()
      {
        Lanes = data.Lanes.Select(o => new LaneDisplayData(o)).ToList(),
        Entries = data.Entries,
      };

      return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(HomeIndexModel data, string? returnurl = null)
    {
      HttpContext.Items.TryGetValue("returnurl", out object? test);

      var userId = this.User.GetId();

      ApiControllers.Data.ClipboardPostPlainTextData apiData = new ApiControllers.Data.ClipboardPostPlainTextData() { Content = data.ContentToAdd };
      await this.repository.PostPlainTextAsync(userId, apiData);

      return LocalRedirect(data.ReturnUrl);
    }

    public IActionResult Privacy()
    {
      return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}