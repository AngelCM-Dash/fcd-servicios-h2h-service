namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Documento.Commands.RechazoDocumentos
{
    using FluentValidation.TestHelper;
    using Interbank.Productos.Comercial.Fcd.Application.Features.Documento.Commands.RechazoDocumentos;
    using Xunit;

    public class RejectionDocumentsValidatorTest
    {
        private readonly RejectionDocumentsValidator _validator;

        public RejectionDocumentsValidatorTest()
        {
            _validator = new RejectionDocumentsValidator();
        }

        // -------------------------------
        // FLAG = 1: Valida todas las reglas
        // -------------------------------

        [Fact]
        public void Debe_Fallar_NumeroPlanilla_Nulo_Flag1()
        {
            var command = new RejectionDocumentsCommand(1, null, "12345678", "Observacion", "01/01/2026");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.NumeroPlanilla);
        }

        [Fact]
        public void Debe_Fallar_NumeroPlanilla_LargoInvalido_Flag1()
        {
            var command = new RejectionDocumentsCommand(1, "123", "12345678", "Observacion", "01/01/2026");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.NumeroPlanilla);
        }

        [Fact]
        public void Debe_Fallar_NumeroLinea_Nulo_Flag1()
        {
            var command = new RejectionDocumentsCommand(1, "1234567890", null, "Observacion", "01/01/2026");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.NumeroLinea);
        }

        [Fact]
        public void Debe_Fallar_NumeroLinea_LargoInvalido_Flag1()
        {
            var command = new RejectionDocumentsCommand(1, "1234567890", "123", "Observacion", "01/01/2026");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.NumeroLinea);
        }

        [Fact]
        public void Debe_Fallar_Observacion_Nula_Flag1()
        {
            var command = new RejectionDocumentsCommand(1, "1234567890", "12345678", null, "01/01/2026");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.Observacion);
        }

        [Fact]
        public void Debe_Fallar_FechaAdelanto_Nula_Flag1()
        {
            var command = new RejectionDocumentsCommand(1, "1234567890", "12345678", "Observacion", null);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.FechaAdelanto);
        }

        [Fact]
        public void Debe_Fallar_FechaAdelanto_FormatoInvalido_Flag1()
        {
            var command = new RejectionDocumentsCommand(1, "1234567890", "12345678", "Observacion", "2026-01-01");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.FechaAdelanto);
        }

        // -------------------------------
        // FLAG = 2: solo validar NumeroPlanilla y Observacion NotEmpty
        // -------------------------------

        [Fact]
        public void Debe_Fallar_NumeroPlanilla_Nulo_Flag2()
        {
            var command = new RejectionDocumentsCommand(2, null, "12345678", "Observacion", "01/01/2026");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.NumeroPlanilla);
        }

        [Fact]
        public void Debe_Fallar_Observacion_Nula_Flag2()
        {
            var command = new RejectionDocumentsCommand(2, "1234567890", "12345678", null, "01/01/2026");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.Observacion);
        }

        [Fact]
        public void No_Debe_Fallar_NumeroLinea_O_Fecha_Flag2()
        {
            var command = new RejectionDocumentsCommand(2, "1234567890", null, "Observacion", null);
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(c => c.NumeroLinea);
            result.ShouldNotHaveValidationErrorFor(c => c.FechaAdelanto);
        }

        // -------------------------------
        // FLAG ≠ 1,2: no valida nada
        // -------------------------------

        [Fact]
        public void No_Debe_Fallar_NingunCampo_Flag3()
        {
            var command = new RejectionDocumentsCommand(3, null, null, null, null);
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(c => c.NumeroPlanilla);
            result.ShouldNotHaveValidationErrorFor(c => c.NumeroLinea);
            result.ShouldNotHaveValidationErrorFor(c => c.Observacion);
            result.ShouldNotHaveValidationErrorFor(c => c.FechaAdelanto);
        }

        [Fact]
        public void EsFechaValidaDDMMYYYY_RetornaFalse_Cuando_Fecha_Nula()
        {
            // Arrange
            string? fecha = null;

            // Act
            var resultado = typeof(RejectionDocumentsValidator)
                .GetMethod("EsFechaValidaDDMMYYYY", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                .Invoke(null, new object?[] { fecha }) as bool?;


            // Assert
            Assert.False(resultado ?? false);
        }

        [Fact]
        public void EsFechaValidaDDMMYYYY_RetornaFalse_Cuando_Fecha_Vacia()
        {
            string fecha = "";

            var resultado = typeof(RejectionDocumentsValidator)
                .GetMethod("EsFechaValidaDDMMYYYY", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                .Invoke(null, new object?[] { fecha }) as bool?;

            // Assert
            Assert.False(resultado ?? false);
        }

    }

}
