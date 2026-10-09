using FluentValidation.TestHelper;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.DesembolsoPlanillasDiferidas;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Desembolso.Commands.DesembolsoPlanillasDiferidas
{
    public class DesembolsoPlanillasDiferidasValidatorTest
    {
        private readonly DesembolsoPlanillasDiferidasValidator _validator;

        public DesembolsoPlanillasDiferidasValidatorTest()
        {
            _validator = new DesembolsoPlanillasDiferidasValidator();
        }

        [Fact]
        public void Valida_NumeroPlanilla_Nulo_O_Vacio()
        {
            var model = new DesembolsoPlanillasDiferidasCommand
            {
                numeroPlanilla = null
            };

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.numeroPlanilla)
                  .WithErrorMessage("{NumeroPlanilla} no puede ser vacio o nulo");
        }

        [Fact]
        public void Valida_NumeroPlanilla_LongitudIncorrecta()
        {
            var model = new DesembolsoPlanillasDiferidasCommand
            {
                numeroPlanilla = "123" // menos de 10
            };

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.numeroPlanilla)
                  .WithErrorMessage("{NumeroPlanilla} tiene que ser de 10 digitos");
        }

        [Fact]
        public void Valida_NumeroPlanilla_Correcto()
        {
            var model = new DesembolsoPlanillasDiferidasCommand
            {
                numeroPlanilla = "1234567890"
            };

            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.numeroPlanilla);
        }
    }
}
