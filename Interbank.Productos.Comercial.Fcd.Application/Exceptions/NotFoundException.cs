namespace Interbank.Productos.Comercial.Fcd.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException()
            : base()
        {
        }

        public NotFoundException(string message)
            : base(message)
        {
        }

        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public NotFoundException(string name, object key)
            : base($"{name} ({key})")
        {
        }

        public NotFoundException(string name, object key, string message)
            : base($"Entity '{name}' ('{key}'): {message}")
        {
        }

    }
}
