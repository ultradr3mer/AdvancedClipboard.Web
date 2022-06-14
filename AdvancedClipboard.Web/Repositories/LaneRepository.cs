using AdvancedClipboard.Web.ApiControllers.Data;
using AdvancedClipboard.Web.Models;

namespace AdvancedClipboard.Web.Repositories
{
  public class LaneRepository
  {
    private readonly ApplicationDbContext context;

    public LaneRepository(ApplicationDbContext context)
    {
      this.context = context;
    }

    internal async Task<LaneGetData> PostAsync(LanePostData data, Guid userId)
    {
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

    public static LaneGetData CreateGetDataFromEntity(LaneEntity lane)
    {
      return new LaneGetData()
      {
        Id = lane.Id,
        Name = lane.Name,
        Color = lane.Color
      };
    }
  }
}
