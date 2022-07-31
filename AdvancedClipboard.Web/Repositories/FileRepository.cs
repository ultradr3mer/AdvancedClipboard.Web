using AdvancedClipboard.Web.Models;
using AdvancedClipboard.Web.Models.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Cryptography;
using AdvancedClipboard.Web;
using AdvancedClipboard.Web.ApiControllers.Data;
using AdvancedClipboard.Server.Constants;

namespace AdvancedClipboard.Server.Repositories
{
  /// <summary>
  /// The file repository.
  /// </summary>
  public class FileRepository
  {
    #region Fields

    private const string UserContainerPrefix = "user-";

    private readonly Dictionary<string, string> mimeExtensions = new Dictionary<string, string>();

    ApplicationDbContext context;
    private readonly BlobServiceClient client;

    #region Constructors

    public FileRepository(ApplicationDbContext context, BlobServiceClient client)
    {
      this.context = context;
      this.client = client;

      this.InitializeMimeTypes(Resource.MimeTypes);
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Creates the token for the file or retrives the existing one.
    /// </summary>
    /// <param name="fileName">The filename.</param>
    /// <param name="connection">The connection.</param>
    /// <returns>The token.</returns>
    public async Task<FileAccessTokenEntity> CreateTokenIfNotExists(string fileName, ApplicationDbContext context, Guid userId)
    {
      FileAccessTokenEntity existingToken = await this.TryUpdateToken(context, userId, fileName);
      if (existingToken != null)
      {
        return existingToken;
      }

      FileAccessTokenEntity newToken = await CreateToken(fileName, userId, context);

      return newToken;
    }

    /// <summary>
    /// Retrives the corresponding content type for the file extension.
    /// </summary>
    /// <param name="extension">The file extension.</param>
    /// <returns>The content type.</returns>
    public string GetContentTypeForExtension(string extension)
    {
      return this.mimeExtensions[extension];
    }

    /// <summary>
    /// Gets the file coresponding to the token and filename.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="filename">The filename.</param>
    /// <returns>The container.</returns>
    public async Task<BlobContainerClient> GetFile(long token, string filename)
    {
      Guid id = await (from t in context.FileAccessToken
                       join u in context.Users on t.UserId equals u.Id
                       where t.Token == token
                       && t.Filename == filename
                       select (Guid?)u.Id).FirstOrDefaultAsync() ?? throw new Exception("File not Found");

      return await this.GetAzureContainer(id);
    }

    /// <summary>
    /// The internal upload function.
    /// </summary>
    /// <param name="filename">The filename of the file to upload.</param>
    /// <param name="content">The content of the file to upload.</param>
    /// <param name="connection">The sql connection.</param>
    /// <param name="overwrite">True, if an existing file should be overwriten.</param>
    /// <returns>The token data of the uploaded file.</returns>
    public async Task<FileAccessTokenEntity> UploadInternal(string filename, Stream content, Guid userId, ApplicationDbContext context, bool overwrite)
    {
      GetContentTypeForExtension(Path.GetExtension(filename));

      FileAccessTokenEntity tokenEntity = await this.CreateTokenIfNotExists(filename, context, userId);
      BlobContainerClient azureContainer = await this.GetAzureContainer(userId);
      BlobClient blob = azureContainer.GetBlobClient(filename);
      await blob.UploadAsync(content, overwrite);

      return tokenEntity;
    }

    private static async Task<FileAccessTokenEntity> CreateToken(string fileName, Guid userId, ApplicationDbContext context)
    {
      FileAccessTokenEntity token = new FileAccessTokenEntity()
      {
        Token = GenerateToken(),
        Filename = fileName,
        UserId = userId
      };

      await context.FileAccessToken.AddAsync(token);
      await context.SaveChangesAsync();

      return token;
    }

    private static long GenerateToken()
    {
      byte[] tokenBytes = RandomNumberGenerator.GetBytes(8);
      long token = BitConverter.ToInt64(tokenBytes);
      return token;
    }

    private async Task<BlobContainerClient> GetAzureContainer(Guid userid)
    {
      string containerName = userid.ToString("N");
      var containerClient = this.client.GetBlobContainerClient(containerName);
      await containerClient.CreateIfNotExistsAsync();

      return containerClient;
    }

    private void InitializeMimeTypes(string mimeTypes)
    {
      string[] lines = mimeTypes.Split(System.Environment.NewLine);

      foreach (string line in lines)
      {
        if (string.IsNullOrEmpty(line))
        {
          continue;
        }

        string[] lineparts = line.Split(',');

        string mime = lineparts[0];
        string extension = lineparts[1];
        string extensionAlt = lineparts[2];

        this.mimeExtensions.Add(extension, mime);

        if (!string.IsNullOrEmpty(extensionAlt))
        {
          this.mimeExtensions.Add(extensionAlt, mime);
        }
      }
    }
    private async Task<FileAccessTokenEntity> TryUpdateToken(ApplicationDbContext context, Guid userId, string fileName)
    {
      FileAccessTokenEntity? token = await (from t in context.FileAccessToken
                                            where t.Filename == fileName
                                            && t.UserId == userId
                                            select t).FirstOrDefaultAsync();

      if (token != null)
      {
        token.Token = GenerateToken();
        await context.SaveChangesAsync();
      }

      return token;
    }

    public async Task<ClipboardGetData> PostFileInternal(IFormFile file, Guid userId, string? fileExtension, string? fileName, Guid? laneId)
    {
      DateTime now = DateTime.Now;
      string extension = (fileExtension ?? Path.GetExtension(fileName) ?? Path.GetExtension(file.FileName));
      string filename = $"clip_{now:yyyyMMdd'_'HHmmss}" + extension;
      FileAccessTokenEntity token = await this.UploadInternal(filename,
                                                              file.OpenReadStream(),
                                                              userId,
                                                              this.context,
                                                              false);

      Guid contentType = this.GetContentTypeForExtension(extension).StartsWith("image") ? ContentTypes.Image :
                                                                                          ContentTypes.File;

      ClipboardContentEntity entry = new ClipboardContentEntity()
      {
        ContentTypeId = contentType,
        CreationDate = now,
        LastUsedDate = now,
        FileTokenId = token.Id,
        UserId = userId,
        DisplayFileName = fileName,
        LaneId = laneId
      };

      await context.AddAsync(entry);
      await context.SaveChangesAsync();

      return contentType == ContentTypes.Image ? ClipboardGetData.CreateWithImageContent(entry.Id, entry.LaneId, token, fileName) :
                                                 ClipboardGetData.CreateWithFileContent(entry.Id, entry.LaneId, token, fileName);
    }

    #endregion Methods
  }

  #endregion
}