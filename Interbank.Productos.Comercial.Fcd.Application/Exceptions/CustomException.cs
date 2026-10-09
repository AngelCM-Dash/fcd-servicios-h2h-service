namespace Interbank.Productos.Comercial.Fcd.Application.Exceptions
{
    public class CustomException : Exception
    {
        public int StatusCode { get; }
        public int CodigoError { get; }
        public CustomException(string message) : base(message)
        {
        }

        public CustomException(int codigoError, string message) : base(message)
        {
            CodigoError = codigoError;
        }

        public CustomException(int codigoError, string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
            CodigoError = codigoError;
        }
    }
}
