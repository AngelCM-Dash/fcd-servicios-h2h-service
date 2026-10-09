using FluentValidation.Results;

namespace Interbank.Productos.Comercial.Fcd.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException() : base("Se presentaron uno o mas errores de validacion")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(Dictionary<string, List<string>> errors)
        : base("Se presentaron uno o más errores de validación")
        {
            Errors = errors.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray());
        }

        public ValidationException(IEnumerable<ValidationFailure> failures) : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }

        public IDictionary<string, string[]> Errors { get; }
    }
}
