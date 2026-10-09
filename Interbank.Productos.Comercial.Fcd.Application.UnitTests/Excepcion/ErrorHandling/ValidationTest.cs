using FluentAssertions;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions.ErrorHandling;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Excepcion.ErrorHandling
{
    public class ValidationTest
    {
        [Fact]
        public void AddValidationFailure_Should_AccumulateErrors_Fluent()
        {
            // Arrange
            var validation = new Validation();
            var propertyName = "CodigoUnico";
            var errorMessage = "El código es requerido";

            // Act
            validation.AddValidationFailure(propertyName, errorMessage);

            // Assert
            Action act = () => validation.ValidationExceptionIfThereAreErrors();

            var exception = act.Should().Throw<ValidationException>().And;

            exception.Errors.Should().ContainKey(propertyName);
            exception.Errors[propertyName].Should().Contain(errorMessage);
        }

        [Fact]
        public void ValidationExceptionIfThereAreErrors_WhenNoErrors_ShouldNotThrow()
        {
            // Arrange
            var validation = new Validation();

            // Act
            Action act = () => validation.ValidationExceptionIfThereAreErrors();

            // Assert (Branch Coverage: case Count == 0)
            act.Should().NotThrow();
        }

        [Fact]
        public void ValidationExceptionIfThereAreErrors_WhenMultipleErrors_ShouldThrowAll()
        {
            // Arrange
            var validation = new Validation();
            validation.AddValidationFailure("Prop1", "Error1");
            validation.AddValidationFailure("Prop2", "Error2");

            // Act
            Action act = () => validation.ValidationExceptionIfThereAreErrors();

            // Assert (Branch Coverage: case Count > 0)
            act.Should().Throw<ValidationException>()
                .And.Errors.Should().HaveCount(2);
        }
    }
}
