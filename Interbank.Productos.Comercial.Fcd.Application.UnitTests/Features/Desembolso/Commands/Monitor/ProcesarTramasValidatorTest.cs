using FluentValidation.TestHelper;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.Monitor;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Desembolso.Commands.Monitor
{
    public class ProcesarTramasValidatorTest
    {
        private readonly ProcesarTramasValidator _validator;

        public ProcesarTramasValidatorTest()
        {
            _validator = new ProcesarTramasValidator();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void FlagMonitor_Should_Not_Have_Error_When_HasValue(bool flag)
        {
            // Arrange
            var command = new ProcesarTramasCommand { FlagMonitor = flag };

            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(c => c.FlagMonitor);
        }

        [Fact]
        public void FlagMonitor_Should_Have_Error_When_Null()
        {
            // Arrange
            var command = new ProcesarTramasCommand { FlagMonitor = null };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c.FlagMonitor)
                  .WithErrorMessage("El valor de Flag Monitor debe ser 'true' o 'false', no se permite otro valor.");
        }
    }
}
