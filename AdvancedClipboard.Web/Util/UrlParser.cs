using Microsoft.AspNetCore.Html;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace AdvancedClipboard.Web.Util
{
  public static class UrlParser
  {
    public static IHtmlContent ConvertToHtmlWithLinks(string text)
    {
      if (text == null)
      {
        return HtmlString.Empty;
      }

      Regex regex = new Regex(@"(https?:\/\/[^\s]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
      System.Collections.Generic.List<string> matches = regex.Matches(text).Cast<Match>().Select(m => m.Value).ToList();

      var matchCollection = regex.Matches(text);
      if(matchCollection.Count == 0)
      {
        return new HtmlString(Encode(text));
      }

      int index = 0;
      StringBuilder sb = new StringBuilder();
      foreach (Match match in matchCollection)
      {
        sb.Append(Encode(text.Substring(index, match.Index - index)));
        sb.Append($"<a href=\"{match.Value}\">{Encode(match.Value)}</a>");
        index = match.Index + match.Length;
      }

      sb.Append(Encode(text.Substring(index, text.Length - index)));
      return new HtmlString(sb.ToString());
    }

    private static string Encode(string text)
    {
      return HttpUtility.HtmlEncode(text).Replace("\r\n", "<br />").Replace("\n", "<br />");
    }
  }
}
