using System;

namespace AdvancedClipboard.Web.ApiControllers.Data
{
  public class AssignContentToLanePutData
  {
    #region Properties

    public Guid ContentId { get; set; }
    public Guid LaneId { get; set; }

    #endregion Properties
  }
}