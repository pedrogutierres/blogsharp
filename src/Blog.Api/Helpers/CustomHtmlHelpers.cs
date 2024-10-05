using System.Text.RegularExpressions;

namespace Blog.Api.Helpers
{
    public static class CustomHtmlHelpers
    {
        public static string RemoverTagsHtml(string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }
    }
}
