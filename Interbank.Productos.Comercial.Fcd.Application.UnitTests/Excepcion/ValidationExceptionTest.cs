using Interbank.Productos.Comercial.Fcd.Application.Exceptions;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Excepcion
{
    public class ValidationExceptionTest
    {
        private static readonly string[] NameErrors = { "Required", "Too short" };
        private static readonly string[] AgeErrors = { "Must be over 18" };

        [Fact]
        public void ValidationException_Should_Map_Errors_Correctly()
        {
            // Arrange
            var errors = new Dictionary<string, List<string>>
            {
                { "Name", new List<string>(NameErrors) },
                { "Age", new List<string>(AgeErrors) }
            };

            // Act
            var exception = new ValidationException(errors);

            // Assert
            Assert.Equal("Se presentaron uno o más errores de validación", exception.Message);

            Assert.True(exception.Errors.ContainsKey("Name"));
            Assert.Equal(NameErrors, exception.Errors["Name"]);

            Assert.True(exception.Errors.ContainsKey("Age"));
            Assert.Equal(AgeErrors, exception.Errors["Age"]);
        }

    }
}
