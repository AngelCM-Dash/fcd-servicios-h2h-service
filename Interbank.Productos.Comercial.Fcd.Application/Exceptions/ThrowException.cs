namespace Interbank.Productos.Comercial.Fcd.Application.Exceptions
{
    public class ThrowException : Exception
    {
        public string? StatusCode { get; }
        public ThrowException(string message) : base(message)
        {
        }

        public ThrowException(string statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public ThrowException(string statusCode, string message, Exception inner) : base(message)
        {
            StatusCode = statusCode;

        }


    }
}
