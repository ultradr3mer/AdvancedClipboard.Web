using AdvancedClipboard.Web.Models;
using AdvancedClipboard.Web.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdvancedClipboard.Web.Extensions;
using AdvancedClipboard.Server.Constants;
using AdvancedClipboard.Server.Repositories;
using Microsoft.Identity.Web;
using AdvancedClipboard.Web.Repositories;
using AdvancedClipboard.Web.ApiControllers.Data;

namespace AdvancedClipboard.Web.ApiControllers
{
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class ClipboardController : ControllerBase
  {
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly FileRepository fileRepository;
    ApplicationDbContext context;
    private readonly ClipboardRepository clipboardRepository;

    #region Constructors

    public ClipboardController(SignInManager<ApplicationUser> _signInManager, FileRepository fileRepository, ApplicationDbContext context, ClipboardRepository clipboardRepository)
    {
      signInManager = _signInManager;
      this.fileRepository = fileRepository;
      this.context = context;
      this.clipboardRepository = clipboardRepository;
    }

    #endregion Constructors

    #region Methods

    [HttpDelete]
    public async Task<ActionResult> DeleteAsync(Guid Id)
    {
      var userId = this.User.GetId();

      await this.clipboardRepository.Delete(Id, userId);

      return this.Ok();
    }

    [HttpGet(nameof(GetLaneWithContext))]
    public async Task<ClipboardContainerGetData> GetLaneWithContext(Guid lane)
    {
      if (lane == Guid.Empty)
      {
        throw new ArgumentNullException(nameof(lane));
      }

      var userId = this.User.GetId();
      ClipboardContainerGetData result = await this.clipboardRepository.GetLaneWithContextAsync(lane, userId);

      return result;
    }


    [HttpGet(nameof(GetWithContext))]
    public async Task<ClipboardContainerGetData> GetWithContext()
    {
      var userId = this.User.GetId();

      var result = await this.clipboardRepository.GetWithContextAsync(null, userId);

      return result;
    }

    [HttpPost("PostFile")]
    public async Task<ClipboardGetData> PostFile(IFormFile file, string fileExtension, Guid? laneId = null)
    {
      var userId = this.User.GetId();

      return await this.fileRepository.PostFileInternal(file, userId, fileExtension, null, laneId);
    }

    [HttpPost("PostNamedFile")]
    public async Task<ClipboardGetData> PostNamedFile(IFormFile file, string fileName, Guid? laneId = null)
    {
      var userId = this.User.GetId();

      return await this.fileRepository.PostFileInternal(file, userId, null, fileName, laneId);
    }

    [HttpPost("PostPlainText")]
    public async Task<ClipboardGetData> PostPlainText(ClipboardPostPlainTextData data)
    {
      var userId = this.User.GetId();

      return await this.clipboardRepository.PostPlainTextAsync(userId, data);
    }

    [HttpPut]
    public async Task<IActionResult> Put(ClipboardPutData data)
    {
      var userId = this.User.GetId();

      await this.clipboardRepository.Put(data, userId);

      return this.Ok();
    }

    #endregion Methods
  }
}