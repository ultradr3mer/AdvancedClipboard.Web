using AdvancedClipboard.Web.Data;
using AdvancedClipboard.Web.Extensions;
using AdvancedClipboard.Web.Models;
using AdvancedClipboard.Web.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdvancedClipboard.Web.ApiControllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class LaneController : Controller
    {
        #region Fields

        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ApplicationDbContext context;

        #endregion Fields

        #region Constructors

        public LaneController(SignInManager<ApplicationUser> _signInManager, ApplicationDbContext context)
        {
            this.signInManager = _signInManager;
            this.context = context;
        }

        #endregion Constructors

        #region Methods

        public static async Task<List<LaneGetData>> GetLanesForUser(ApplicationDbContext context, Guid userId)
        {
            return await (from lane in context.Lane
                          where lane.UserId == userId
                          select CreateGetDataFromEntity(lane)).ToListAsync();
        }

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

            LaneEntity lane = await context.Lane.FindAsync(laneId) ?? throw new Exception("Lane not Found");
            if (lane.UserId != userId)
            {
                throw new Exception("Lane not Found");
            }

            context.Remove(lane);

            await context.SaveChangesAsync();

            return this.Ok();
        }

        [HttpGet]
        public async Task<IEnumerable<LaneGetData>> Get()
        {
            var userId = this.User.GetId();

            List<LaneGetData> result = await GetLanesForUser(context, userId);

            return result;
        }

        [HttpPost("PostLane")]
        public async Task<LaneGetData> Post(LanePostData data)
        {
            var userId = this.User.GetId();

            LaneEntity entity = new LaneEntity()
            {
                Name = data.Name,
                Color = data.Color,
                UserId = userId
            };

            await context.AddAsync(entity);
            await context.SaveChangesAsync();

            return CreateGetDataFromEntity(entity);
        }

        [HttpPut]
        public async Task<ActionResult> Put(LanePutData data)
        {
            var userId = this.User.GetId();

            LaneEntity lane = await context.Lane.FindAsync(data.Id) ?? throw new Exception("Lane not Found");
            if (lane.UserId != userId)
            {
                throw new Exception("Lane not Found");
            }

            lane.Name = data.Name;
            lane.Color = data.Color;

            await context.SaveChangesAsync();

            return this.Ok();
        }

        private static LaneGetData CreateGetDataFromEntity(LaneEntity lane)
        {
            return new LaneGetData()
            {
                Id = lane.Id,
                Name = lane.Name,
                Color = lane.Color
            };
        }

        #endregion Methods
    }
}