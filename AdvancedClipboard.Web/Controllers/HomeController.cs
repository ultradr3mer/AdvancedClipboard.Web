﻿using AdvancedClipboard.Web.ApiControllers;
using AdvancedClipboard.Web.Data;
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

            var data = await this.repository.GetWithContext(id, userId);

            return View(data);
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