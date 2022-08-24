using System;

namespace AdvancedClipboard.Web.ApiControllers.Data
{
  public class LaneGetData
  {
    #region Properties

    public string Color { get; set; } = string.Empty;
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool HasItems { get; set; }

    #endregion Properties
  }
}