using System.Text.RegularExpressions;

namespace Blog.Web.Helpers
{
    public static class CustomHtmlHelpers
    {
        public static string RemoverTagsHtml(string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }
    }
}
