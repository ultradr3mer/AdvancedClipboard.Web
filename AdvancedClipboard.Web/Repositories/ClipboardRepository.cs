using AdvancedClipboard.Server.Constants;
using AdvancedClipboard.Web.ApiControllers;
using AdvancedClipboard.Web.ApiControllers.Data;
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

    internal async Task<ClipboardContainerGetData> GetWithContextAsync(Guid? id, Guid userId)
    {
      List<ClipboardGetData> entries = await (from cc in this.context.ClipboardContent
                                              where cc.UserId == userId
                                              && cc.IsArchived == false
                                              && (cc.Id == id || id == null)
                                              select new
                                              {
                                                data = ClipboardGetData.CreateFromEntity(cc, cc.FileToken),
                                                date = cc.CreationDate
                                              })
                                             .OrderByDescending(o => o.date).Select(o => o.data).ToListAsync();

      List<LaneGetData> lanes = await LaneController.GetLanesForUser(this.context, userId);

      ClipboardContainerGetData result = new ClipboardContainerGetData()
      {
        Entries = entries,
        Lanes = lanes
      };

      return result;
    }

    internal async Task<ClipboardGetData> PostPlainTextAsync(Guid userId, ClipboardPostPlainTextData data)
    {
      DateTime now = DateTime.Now;
      ClipboardContentEntity entry = new ClipboardContentEntity()
      {
        ContentTypeId = ContentTypes.PlainText,
        CreationDate = now,
        LastUsedDate = now,
        TextContent = data.Content,
        UserId = userId,
        LaneId = data.LaneGuid
      };

      await this.context.AddAsync(entry);
      await this.context.SaveChangesAsync();

      return ClipboardGetData.CreateWithPlainTextContent(entry.Id, entry.LaneId, entry.TextContent);
    }

    internal async Task<ClipboardContainerGetData> GetLaneWithContextAsync(Guid lane, Guid userId)
    {
      List<ClipboardGetData> entries = await (from cc in context.ClipboardContent
                                              where cc.UserId == userId
                                              && cc.IsArchived == false
                                              && cc.LaneId == lane
                                              select ClipboardGetData.CreateFromEntity(cc, cc.FileToken)).ToListAsync();

      List<LaneGetData> lanes = await LaneController.GetLanesForUser(context, userId);

      ClipboardContainerGetData result = new ClipboardContainerGetData()
      {
        Entries = entries,
        Lanes = lanes
      };
      return result;
    }

    internal async Task Put(ClipboardPutData data, Guid userId)
    {
      ClipboardContentEntity cc = await context.ClipboardContent.FindAsync(data.Id) ?? throw new Exception("Content to Update not found.");

      if (cc.UserId != userId)
      {
        throw new Exception("Content to Update not found.");
      }

      cc.DisplayFileName = data.FileName;
      cc.TextContent = data.TextContent;
      cc.LaneId = data.LaneId;

      await context.SaveChangesAsync();
    }

    internal async Task Delete(Guid id, Guid userId)
    {
      var cc = await context.ClipboardContent.FindAsync(id) ?? throw new Exception("Item Not Found.");
      cc.IsArchived = true;

      if (cc.UserId != userId)
      {
        throw new Exception("Item Not Found.");
      }

      await context.SaveChangesAsync();
    }

  }
}
