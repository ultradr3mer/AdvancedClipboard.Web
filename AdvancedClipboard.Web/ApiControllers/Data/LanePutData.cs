﻿using System;

namespace AdvancedClipboard.Web.ApiControllers.Data
{
  public class LanePutData
  {
    #region Properties

    public string Color { get; set; } = string.Empty;
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    #endregion Properties
  }
}