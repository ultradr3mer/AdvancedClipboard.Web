using AdvancedClipboard.Web.Controllers.Model;
using AdvancedClipboard.Web.Extensions;
using AdvancedClipboard.Web.Models;
using AdvancedClipboard.Web.Repositories;
using AdvancedClipboard.Web.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AdvancedClipboard.Web.Controllers
{
  [Authorize]
  public class HomeController : Controller
  {
    private readonly ClipboardRepository repository;
    private readonly MimeTypeResolver mimeTypeResolver;

    public HomeController(ClipboardRepository clipboardController, MimeTypeResolver mimeTypeResolver)
    {
      this.repository = clipboardController;
      this.mimeTypeResolver = mimeTypeResolver;
    }

    [HttpGet("Lane")]
    public async Task<IActionResult> Lane(Guid laneid)
    {
      var userId = this.User.GetId();

      var data = await this.repository.GetLaneWithContextAsync(laneid, userId);

      var model = new HomeIndexModel()
      {
        Lanes = data.Lanes.Select(o => new LaneDisplayData(o)).ToList(),
        Entries = data.Entries,
        CurrentLaneId = laneid,
        FileFilter = string.Join(", ", mimeTypeResolver.GetAllExtensions())
      };

      return View(nameof(Index), model);
    }

    public async Task<IActionResult> Index()
    {
      var userId = this.User.GetId();

      var data = await this.repository.GetWithContextAsync(null, userId);

      var model = new HomeIndexModel()
      {
        Lanes = data.Lanes.Select(o => new LaneDisplayData(o)).ToList(),
        Entries = data.Entries,
        FileFilter = string.Join(", ", mimeTypeResolver.GetAllExtensions())
      };

      return View(model);
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