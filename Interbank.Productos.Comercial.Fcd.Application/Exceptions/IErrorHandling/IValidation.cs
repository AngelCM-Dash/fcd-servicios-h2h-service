namespace Interbank.Productos.Comercial.Fcd.Application.Exceptions.IErrorHandling
{
    public interface IValidation
    {
        void AddValidationFailure(string propertyName, string errorMessage);

        void ValidationExceptionIfThereAreErrors();
    }
}
