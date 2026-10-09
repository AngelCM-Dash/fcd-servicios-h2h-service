using FluentValidation.TestHelper;
using Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.Encolamiento;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Planilla.Commands.Encolamiento
{
    public class EncolarPlanillaValidatorTest
    {
        private readonly EncolarPlanillaValidator _validator;

        public EncolarPlanillaValidatorTest()
        {
            _validator = new EncolarPlanillaValidator();
        }

        private static EncolarPlanillaCommand CrearCommandValido()
        {
            return new EncolarPlanillaCommand
            {
                CodigoProducto = 31,
                CanalAtencion = "CanalX",
                CodigoUsuario = 123,
                Usuario = "UsuarioTest",
                CodigoPerfilUsuario = 1,
                CodigoTienda = 1,
                Tienda = "TiendaX",
                ContratoMarco = true,
                RutaArchivo = "/ruta/archivo.xlsx",
                NombreArchivo = "archivo.xlsx",
                CodigoUnico = "UNICO123",
                FechaValor = DateTime.Today.AddDays(1),
                Adicional = [],
            };
        }

        // ----------------------------
        // CodigoProducto
        // ----------------------------
        [Theory]
        [InlineData(0)]
        [InlineData(30)]
        [InlineData(35)]
        public void CodigoProducto_Invalido_Falla(int codigo)
        {
            var command = CrearCommandValido();
            command.CodigoProducto = codigo;

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.CodigoProducto);
        }

        [Fact]
        public void CodigoProducto_Valido_NoFalla()
        {
            var command = CrearCommandValido();
            command.CodigoProducto = 31;

            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(c => c.CodigoProducto);
        }

        // ----------------------------
        // CanalAtencion
        // ----------------------------
        [Fact]
        public void CanalAtencion_Vacio_Falla()
        {
            var command = CrearCommandValido();
            command.CanalAtencion = "";

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.CanalAtencion);
        }

        // ----------------------------
        // CodigoUsuario
        // ----------------------------
        [Fact]
        public void CodigoUsuario_Cero_Falla()
        {
            var command = CrearCommandValido();
            command.CodigoUsuario = 0;

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.CodigoUsuario);
        }

        // ----------------------------
        // Usuario
        // ----------------------------
        [Fact]
        public void Usuario_Vacio_Falla()
        {
            var command = CrearCommandValido();
            command.Usuario = "";

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.Usuario);
        }

        // ----------------------------
        // CodigoPerfilUsuario
        // ----------------------------
        [Fact]
        public void CodigoPerfilUsuario_Cero_Falla()
        {
            var command = CrearCommandValido();
            command.CodigoPerfilUsuario = 0;

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.CodigoPerfilUsuario);
        }

        [Fact]
        public void CodigoPerfilUsuario_Valido_NoFalla()
        {
            var command = CrearCommandValido();
            command.CodigoPerfilUsuario = 1;

            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(c => c.CodigoPerfilUsuario);
        }

        // ----------------------------
        // CodigoTienda
        // ----------------------------
        [Fact]
        public void CodigoTienda_Cero_Falla()
        {
            var command = CrearCommandValido();
            command.CodigoTienda = 0;

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.CodigoTienda);
        }

        [Fact]
        public void CodigoTienda_Valido_NoFalla()
        {
            var command = CrearCommandValido();
            command.CodigoTienda = 1;

            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(c => c.CodigoTienda);
        }

        // ----------------------------
        // Tienda
        // ----------------------------
        [Fact]
        public void Tienda_Vacio_Falla()
        {
            var command = CrearCommandValido();
            command.Tienda = "";

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.Tienda);
        }

        // ----------------------------
        // ContratoMarco
        // ----------------------------
        [Fact]
        public void ContratoMarco_Nulo_Falla()
        {
            var command = CrearCommandValido();
            command.ContratoMarco = null;

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.ContratoMarco);
        }

        // ----------------------------
        // RutaArchivo
        // ----------------------------
        [Fact]
        public void RutaArchivo_Vacio_Falla()
        {
            var command = CrearCommandValido();
            command.RutaArchivo = "";

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.RutaArchivo);
        }

        // ----------------------------
        // NombreArchivo
        // ----------------------------
        [Fact]
        public void NombreArchivo_Vacio_Falla()
        {
            var command = CrearCommandValido();
            command.NombreArchivo = "";

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.NombreArchivo);
        }

        // ----------------------------
        // CodigoUnico
        // ----------------------------
        [Fact]
        public void CodigoUnico_Vacio_Falla()
        {
            var command = CrearCommandValido();
            command.CodigoUnico = "";

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.CodigoUnico);
        }

        // ----------------------------
        // FechaValor
        // ----------------------------
        [Fact]
        public void FechaValor_Nula_Falla()
        {
            var command = CrearCommandValido();
            command.FechaValor = null;

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.FechaValor);
        }

        [Fact]
        public void FechaValor_Pasada_Falla()
        {
            var command = CrearCommandValido();
            command.FechaValor = DateTime.Today.AddDays(-1);

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.FechaValor);
        }

        [Fact]
        public void FechaValor_HoyOPosterior_NoFalla()
        {
            var command = CrearCommandValido();
            command.FechaValor = DateTime.Today;

            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(c => c.FechaValor);

            command.FechaValor = DateTime.Today.AddDays(5);
            result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(c => c.FechaValor);
        }
    }
}
