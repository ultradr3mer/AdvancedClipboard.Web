namespace AdvancedClipboard.Web.Util
{
  public class MimeTypeResolver
  {
    private readonly Dictionary<string, string> extensionMimeMap = new Dictionary<string, string>();

    public MimeTypeResolver()
    {
      this.InitializeMimeTypes(Resource.MimeTypes);
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

        this.extensionMimeMap.Add(extension, mime);

        if (!string.IsNullOrEmpty(extensionAlt))
        {
          this.extensionMimeMap.Add(extensionAlt, mime);
        }
      }
    }

    internal string GetMimeType(string extension)
    {
      return this.extensionMimeMap[extension];
    }

    internal IList<string> GetAllExtensions()
    {
      return this.extensionMimeMap.Keys.ToList();
    }
  }
}