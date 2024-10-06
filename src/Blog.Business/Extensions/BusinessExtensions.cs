using System.Text;

namespace Blog.Business.Extensions
{
    public static class BusinessExtensions
    {
        public static string AgruparTodasAsMensagens(this Exception exception)
        {
            var msg = new StringBuilder(exception.Message);

            var tmp = exception;
            while (tmp.InnerException != null)
            {
                tmp = tmp.InnerException;
                msg.AppendLine();
                msg.Append(tmp.Message);
            }

            return msg.ToString();
        }

        public static string AgruparTodasAsException(this Exception exception)
        {
            var msg = new StringBuilder(exception.ToString());

            var tmp = exception;
            while (tmp.InnerException != null)
            {
                tmp = tmp.InnerException;

                msg.AppendLine();
                msg.Append(tmp.ToString());
            }

            return msg.ToString();
        }
    }
}
