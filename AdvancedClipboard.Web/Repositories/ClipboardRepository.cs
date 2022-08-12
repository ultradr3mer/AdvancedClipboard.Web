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
    private readonly LaneRepository laneRepository;

    public ClipboardRepository(ApplicationDbContext context, LaneRepository laneRepository)
    {
      this.context = context;
      this.laneRepository = laneRepository;
    }

    internal async Task<ClipboardContainerGetData> GetWithContextAsync(Guid? id, Guid userId)
    {
      List<ClipboardGetData> entries = await (from cc in this.context.ClipboardContent
                                              where cc.UserId == userId
                                              && cc.IsArchived == false
                                              && (cc.Id == id || id == null)
                                              select new
                                              {
                                                cc = cc,
                                                token = cc.FileToken,
                                                date = cc.CreationDate
                                              })
                                             .OrderByDescending(o => o.date)
                                             .Select(o => ClipboardGetData.CreateFromEntity(o.cc, o.cc.FileToken))
                                             .ToListAsync();

      List<LaneGetData> lanes = await this.laneRepository.GetLanesForUser(userId, null);

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

      return ClipboardGetData.CreateWithPlainTextContent(entry);
    }

    internal async Task<ClipboardContainerGetData> GetLaneWithContextAsync(Guid lane, Guid userId)
    {
      List<ClipboardGetData> entries = await (from cc in context.ClipboardContent
                                              where cc.UserId == userId
                                              && cc.IsArchived == false
                                              && cc.LaneId == lane
                                              select new
                                              {
                                                cc = cc,
                                                token = cc.FileToken,
                                                date = cc.CreationDate
                                              })
                                             .OrderByDescending(o => o.date)
                                             .Select(o => ClipboardGetData.CreateFromEntity(o.cc, o.cc.FileToken))
                                             .ToListAsync();


      List<LaneGetData> lanes = await this.laneRepository.GetLanesForUser(userId, null);

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

    internal async Task Pin(Guid id, Guid userId)
    {
      var cc = await context.ClipboardContent.FindAsync(id) ?? throw new Exception("Item Not Found.");

      if (cc.UserId != userId)
      {
        throw new Exception("Item Not Found.");
      }

      cc.IsPinned = true;

      await context.SaveChangesAsync();
    }

    internal async Task Unpin(Guid id, Guid userId)
    {
      var cc = await context.ClipboardContent.FindAsync(id) ?? throw new Exception("Item Not Found.");

      if (cc.UserId != userId)
      {
        throw new Exception("Item Not Found.");
      }

      cc.IsPinned = false;

      await context.SaveChangesAsync();
    }
  }
}
