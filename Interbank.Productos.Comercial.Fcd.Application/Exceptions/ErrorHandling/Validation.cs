using FluentValidation.Results;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions.IErrorHandling;

namespace Interbank.Productos.Comercial.Fcd.Application.Exceptions.ErrorHandling
{
    public class Validation : IValidation
    {
        private readonly List<ValidationFailure> _validationFailures;

        public Validation()
        {
            _validationFailures = new List<ValidationFailure>();
        }

        public void AddValidationFailure(string propertyName, string errorMessage)
        {
            _validationFailures.Add(new ValidationFailure
            {
                PropertyName = propertyName,
                ErrorMessage = errorMessage
            });
        }

        public void ValidationExceptionIfThereAreErrors()
        {
            if (_validationFailures.Count > 0)
            {
                throw new ValidationException(_validationFailures);
            }
        }
    }
}
