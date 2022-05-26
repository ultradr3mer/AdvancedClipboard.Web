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
      var cc = await context.ClipboardContent.FindAsync(Id) ?? throw new ArgumentException("Item Not Found");
      cc.IsArchived = true;

      if (cc.UserId != this.User.GetId())
      {
        return this.BadRequest();
      }

      await context.SaveChangesAsync();

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
    public async Task<ClipboardContainerGetData> GetWithContext(Guid? id = null)
    {
      var userId = this.User.GetId();

      var result = await this.clipboardRepository.GetWithContextAsync(id, userId);

      return result;
    }

    [HttpPost("PostFile")]
    public async Task<ClipboardGetData> PostFile(IFormFile file, string fileExtension, Guid? laneId = null)
    {
      return await this.PostFileInternal(file, fileExtension, null, laneId);
    }

    [HttpPost("PostNamedFile")]
    public async Task<ClipboardGetData> PostNamedFile(IFormFile file, string fileName, Guid? laneId = null)
    {
      return await this.PostFileInternal(file, null, fileName, laneId);
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
      ClipboardContentEntity cc = await context.ClipboardContent.FindAsync(data.Id) ?? throw new Exception("Content to Update not found.");

      var userId = this.User.GetId();

      if (cc.UserId != userId)
      {
        throw new Exception("Content to Update not found.");
      }

      cc.DisplayFileName = data.FileName;
      cc.TextContent = data.TextContent;
      cc.LaneId = data.LaneId;

      await context.SaveChangesAsync();

      return this.Ok();
    }

    private async Task<ClipboardGetData> PostFileInternal(IFormFile file, string? fileExtension, string? fileName, Guid? laneId)
    {
      var userId = this.User.GetId();
      var userName = this.User.GetNameIdentifierId() ?? this.User.GetDisplayName() ?? throw new Exception("User has no Name");

      DateTime now = DateTime.Now;
      string extension = (fileExtension ?? Path.GetExtension(fileName) ?? Path.GetExtension(file.FileName));
      string filename = $"clip_{now:yyyyMMdd'_'HHmmss}" + extension;
      FileAccessTokenEntity token = await this.fileRepository.UploadInternal(filename,
                                                                             file.OpenReadStream(),
                                                                             userId,
                                                                             userName,
                                                                             this.context,
                                                                             false);

      Guid contentType = this.fileRepository.GetContentTypeForExtension(extension).StartsWith("image") ? ContentTypes.Image :
                                                                                                   ContentTypes.File;

      ClipboardContentEntity entry = new ClipboardContentEntity()
      {
        ContentTypeId = contentType,
        CreationDate = now,
        LastUsedDate = now,
        FileTokenId = token.Id,
        UserId = userId,
        DisplayFileName = fileName,
        LaneId = laneId
      };

      await context.AddAsync(entry);
      await context.SaveChangesAsync();

      return contentType == ContentTypes.Image ? ClipboardGetData.CreateWithImageContent(entry.Id, entry.LaneId, token, fileName) :
                                                           ClipboardGetData.CreateWithFileContent(entry.Id, entry.LaneId, token, fileName);
    }

    #endregion Methods
  }
}