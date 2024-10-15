using System.Collections;

namespace Blog.Application.Exceptions
{
    public class BusinessException : Exception
    {
        public BusinessException(string message)
            : base(message)
        { }
        public BusinessException(string message, Exception innerException)
            : base(message, innerException)
        { }
        public BusinessException(string message, Exception innerException, Dictionary<string, string> data)
            : base(message, innerException)
        {
            _data = data;
        }

        private readonly Dictionary<string, string> _data;
        public override IDictionary Data => _data ?? base.Data;
    }
}
