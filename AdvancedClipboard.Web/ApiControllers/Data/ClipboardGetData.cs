using AdvancedClipboard.Server.Constants;
using AdvancedClipboard.Web.Models;
using System;

namespace AdvancedClipboard.Web.ApiControllers.Data
{
  public class ClipboardGetData
  {
    #region Constructors

    private ClipboardGetData(Guid id)
    {
      Id = id;
    }

    #endregion Constructors

    #region Properties

    public Guid ContentTypeId { get; private set; }
    public string FileContentUrl { get; private set; } = string.Empty;
    public Guid Id { get; }
    public string TextContent { get; private set; } = string.Empty;
    public string FileName { get; private set; } = string.Empty;
    public Guid? LaneId { get; set; }

    #endregion Properties

    #region Methods

    public static ClipboardGetData CreateWithFileContent(Guid id, Guid? laneId, FileAccessTokenEntity fileToken, string fileName)
    {
      return new ClipboardGetData(id) { FileContentUrl = FileTokenData.CreateUrl(fileToken), LaneId = laneId, ContentTypeId = ContentTypes.File, FileName = fileName };
    }

    public static ClipboardGetData CreateWithImageContent(Guid id, Guid? laneId, FileAccessTokenEntity fileToken, string fileName)
    {
      return new ClipboardGetData(id) { FileContentUrl = FileTokenData.CreateUrl(fileToken), LaneId = laneId, ContentTypeId = ContentTypes.Image, FileName = fileName };
    }

    public static ClipboardGetData CreateWithPlainTextContent(Guid id, Guid? laneId, string text)
    {
      return new ClipboardGetData(id) { TextContent = text, LaneId = laneId, ContentTypeId = ContentTypes.PlainText, };
    }

    internal static ClipboardGetData CreateFromEntity(ClipboardContentEntity cc, FileAccessTokenEntity? fileToken)
    {
      var contentType = cc.ContentType?.Id ?? cc.ContentTypeId;

      if (contentType == ContentTypes.Image)
      {
        return CreateWithImageContent(cc.Id, cc.LaneId, fileToken!, cc.DisplayFileName!);
      }
      else if (contentType == ContentTypes.PlainText)
      {
        return CreateWithPlainTextContent(cc.Id, cc.LaneId, cc.TextContent!);
      }
      else if (contentType == ContentTypes.File)
      {
        return CreateWithFileContent(cc.Id, cc.LaneId, fileToken!, cc.DisplayFileName!);
      }

      throw new Exception("Unexpected Content Type");
    }

    #endregion Methods
  }
}