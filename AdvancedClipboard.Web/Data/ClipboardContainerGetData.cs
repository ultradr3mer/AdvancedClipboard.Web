﻿using System.Collections.Generic;

namespace AdvancedClipboard.Web.Data
{
  public class ClipboardContainerGetData
  {
    #region Properties

    public List<LaneGetData> Lanes { get; set; }
    public List<ClipboardGetData> Entries { get; set; }

    #endregion Properties
  }
}