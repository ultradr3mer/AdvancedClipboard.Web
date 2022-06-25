using AdvancedClipboard.Web.ApiControllers.Data;
using AdvancedClipboard.Web.Extensions;
using AdvancedClipboard.Web.Models;
using AdvancedClipboard.Web.Models.Identity;
using AdvancedClipboard.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdvancedClipboard.Web.ApiControllers
{
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class LaneController : Controller
  {
    #region Fields

    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly ApplicationDbContext context;
    private readonly LaneRepository repository;
    private readonly LaneRepository laneRepository;

    #endregion Fields

    #region Constructors

    public LaneController(SignInManager<ApplicationUser> _signInManager,
                          ApplicationDbContext context,
                          LaneRepository repository,
                          LaneRepository laneRepository)
    {
      this.signInManager = _signInManager;
      this.context = context;
      this.repository = repository;
      this.laneRepository = laneRepository;
    }

    #endregion Constructors

    #region Methods

    [HttpPut("AssignContent")]
    public async Task<ActionResult> AssignContent(AssignContentToLanePutData data)
    {
      var userId = this.User.GetId();

      ClipboardContentEntity content = await context.ClipboardContent.FindAsync(data.ContentId) ?? throw new Exception("Clipboard Content not Found");
      if (content.UserId != userId)
      {
        throw new Exception("Clipboard Content not Found");
      }

      content.LaneId = data.LaneId;

      await context.SaveChangesAsync();

      return this.Ok();
    }

    [HttpDelete]
    public async Task<ActionResult> Delete(Guid laneId)
    {
      var userId = this.User.GetId();

      await this.repository.DeleteAsync(userId, laneId);
      return this.Ok();
    }

    [HttpGet]
    public async Task<IEnumerable<LaneGetData>> Get()
    {
      var userId = this.User.GetId();

      List<LaneGetData> result = await this.laneRepository.GetLanesForUser(userId, null);

      return result;
    }

    [HttpGet]
    public async Task<IEnumerable<LaneGetData>> Get(Guid id)
    {
      var userId = this.User.GetId();

      List<LaneGetData> result = await this.laneRepository.GetLanesForUser(userId, null);

      return result;
    }

    [HttpPost("PostLane")]
    public async Task<LaneGetData> Post(LanePostData data)
    {
      var userId = this.User.GetId();

      var result = await this.repository.PostAsync(userId, data);

      return result;
    }

    [HttpPut]
    public async Task<ActionResult> Put(LanePutData data)
    {
      var userId = this.User.GetId();

      await this.repository.PutAsync(userId, data);


      return this.Ok();
    }


    #endregion Methods
  }
}