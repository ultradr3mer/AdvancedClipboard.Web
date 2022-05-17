using AdvancedClipboard.Web.ApiControllers;
using AdvancedClipboard.Web.Data;
using AdvancedClipboard.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AdvancedClipboard.Web.Repositories
{
    public class ClipboardRepository
    {
        private readonly ApplicationDbContext context;

        public ClipboardRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        internal async Task<ClipboardContainerGetData> GetWithContext(Guid? id, Guid userId)
        {
            List<ClipboardGetData> entries = await(from cc in context.ClipboardContent
                                                   where cc.UserId == userId
                                                   && cc.IsArchived == false
                                                   && (cc.Id == id || id == null)
                                                   select new { data = ClipboardGetData
                                                   .CreateFromEntity(cc, cc.FileToken), date = cc.CreationDate })
                                                   .OrderByDescending(o=>o.date).Select(o=>o.data).ToListAsync();

            List<LaneGetData> lanes = await LaneController.GetLanesForUser(context, userId);

            ClipboardContainerGetData result = new ClipboardContainerGetData()
            {
                Entries = entries,
                Lanes = lanes
            };

            return result;
        }
    }
}
