using AdvancedClipboard.Web.ApiControllers.Data;
using AdvancedClipboard.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AdvancedClipboard.Web.Repositories
{
  public class LaneRepository
  {
    private readonly ApplicationDbContext context;

    public LaneRepository(ApplicationDbContext context)
    {
      this.context = context;
    }

    internal async Task<LaneGetData> PostAsync(Guid userId, LanePostData data)
    {
      LaneEntity entity = new LaneEntity()
      {
        Name = data.Name,
        Color = data.Color,
        UserId = userId
      };

      await context.AddAsync(entity);
      await context.SaveChangesAsync();

      return CreateGetDataFromEntity(entity, false);
    }

    public static LaneGetData CreateGetDataFromEntity(LaneEntity lane, bool hasItems)
    {
      return new LaneGetData()
      {
        Id = lane.Id,
        Name = lane.Name,
        Color = lane.Color,
        HasItems = hasItems,
      };
    }

    public async Task<List<LaneGetData>> GetLanesForUser(Guid userId, Guid? id)
    {
      return await (from lane in context.Lane
                    where lane.UserId == userId
                    && (lane.Id == id || id == null)
                    select LaneRepository.CreateGetDataFromEntity(lane, lane.ClipboardContentItems.Any())).ToListAsync();
    }

    internal async Task PutAsync(Guid userId, LanePutData data)
    {
      LaneEntity lane = await context.Lane.FindAsync(data.Id) ?? throw new Exception("Lane not Found");
      if (lane.UserId != userId)
      {
        throw new Exception("Lane not Found");
      }

      lane.Name = data.Name;
      lane.Color = data.Color;

      await context.SaveChangesAsync();
    }

    internal async Task DeleteAsync(Guid userId, Guid laneId)
    {
      LaneEntity lane = await context.Lane.FindAsync(laneId) ?? throw new Exception("Lane not Found");
      if (lane.UserId != userId)
      {
        throw new Exception("Lane not Found");
      }

      var items = (from cc in context.ClipboardContent
                   where cc.LaneId == laneId
                   select cc).ToList();
      foreach (var item in items)
      {
        item.LaneId = null;
        item.Lane = null;
      }

      context.Remove(lane);

      await context.SaveChangesAsync();
    }
  }
}
