using AdvancedClipboard.Web.ApiControllers.Data;
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

    [HttpGet(nameof(Lane))]
    public async Task<IActionResult> Lane(Guid laneid, string? searchText)
    {
      var userId = this.User.GetId();

      var data = await this.repository.GetLaneWithContextAsync(laneid, userId);

      ApplySerachFilter(searchText, data);

      var pinned = new List<ClipboardGetData>();
      var entries = new List<ClipboardGetData>();
      foreach (var entry in data.Entries)
      {
        var target = entry.IsPinned ? pinned : entries;
        target.Add(entry);
      }

      string fullPath = this.Request.Path + this.Request.QueryString;

      var model = new HomeIndexModel()
      {
        Lanes = data.Lanes.Select(o => new LaneDisplayData(o)).ToList(),
        Pinned = pinned,
        Entries = entries,
        CurrentLaneId = laneid,
        FileFilter = string.Join(", ", mimeTypeResolver.GetAllExtensions()),
        FullPath = fullPath
      };
      model.Initialize();

      return View(nameof(Index), model);
    }

    [HttpGet()]
    public async Task<IActionResult> Index(string? searchText)
    {
      var userId = this.User.GetId();

      var data = await this.repository.GetWithContextAsync(null, userId);

      ApplySerachFilter(searchText, data);

      var pinned = new List<ClipboardGetData>();
      var entries = new List<ClipboardGetData>();
      foreach (var entry in data.Entries)
      {
        var target = entry.IsPinned ? pinned : entries;
        target.Add(entry);
      }

      string fullPath = this.Request.Path + this.Request.QueryString;

      var model = new HomeIndexModel()
      {
        Lanes = data.Lanes.Select(o => new LaneDisplayData(o)).ToList(),
        Pinned = pinned,
        Entries = entries,
        FileFilter = string.Join(", ", mimeTypeResolver.GetAllExtensions()),
        FullPath = fullPath
      };
      model.Initialize();

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

    private static void ApplySerachFilter(string? searchText, ClipboardContainerGetData data)
    {
      if (searchText != null)
      {
        data.Entries = data.Entries.Where(o => o.TextContent?.Contains(searchText, StringComparison.OrdinalIgnoreCase) == true
                                          || o.FileName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) == true).ToList();
      }
    }
  }
}