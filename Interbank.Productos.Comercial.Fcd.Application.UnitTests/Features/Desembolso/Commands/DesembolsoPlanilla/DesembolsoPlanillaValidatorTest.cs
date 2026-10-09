using FluentValidation.TestHelper;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.DesembolsoPlanilla;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Desembolso.Commands.DesembolsoPlanilla
{
    public class DesembolsoPlanillaValidatorTest
    {
        private readonly DesembolsoPlanillaValidator _validator;

        public DesembolsoPlanillaValidatorTest()
        {
            _validator = new DesembolsoPlanillaValidator();
        }

        private static DesembolsoPlanillaCommand CrearCommandValido()
        {
            return new DesembolsoPlanillaCommand
            {
                NumeroPlanilla = "1234567890",
                EstadoPlanilla = 15,
                EstadoDocumento = 27,
                CodigoPerfilUsuario = 12345,
                FlagDesembolsoTotal = false,
                CodigoAgrupamiento = "AGRUP1",
                CodigoTienda = "TIENDA001",
                Comentario = "Comentario válido",
                NombreUsuarioRegistro = "UsuarioTest",
                CanalAtencion = "CanalX",
                CodigoUsuario = "USER001",
                CodigoUnico = "UNICO12345",
                CodigoReserva = new List<int> { 1234567891, 1234567891, 1234567891 }
            };
        }

        [Fact]
        public void Validator_Deberia_Pasar_Cuando_Todos_Los_Campos_Son_Validos()
        {
            var command = CrearCommandValido();
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        #region NumeroPlanilla
        [Fact]
        public void NumeroPlanilla_Debe_Fallar_Si_Esta_Vacio()
        {
            var command = CrearCommandValido();
            command.NumeroPlanilla = "";
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.NumeroPlanilla)
                  .WithErrorMessage("{NumeroPlanilla} no puede ser vacio o nulo");
        }

        [Fact]
        public void NumeroPlanilla_Debe_Fallar_Si_No_Tiene_10_Digitos()
        {
            var command = CrearCommandValido();
            command.NumeroPlanilla = "123";
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.NumeroPlanilla)
                  .WithErrorMessage("{NumeroPlanilla} tiene que ser de 10 digitos");
        }
        #endregion

        #region EstadoPlanilla
        [Fact]
        public void EstadoPlanilla_Debe_Fallar_Si_Esta_Vacio()
        {
            var command = CrearCommandValido();
            command.EstadoPlanilla = null; // Considerando que NotEmpty en int falla si es 0
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.EstadoPlanilla)
                  .WithErrorMessage("{estadoPlanilla} no puede ser vacio o nulo");
        }

        [Fact]
        public void EstadoPlanilla_Debe_Fallar_Si_No_Es_15()
        {
            var command = CrearCommandValido();
            command.EstadoPlanilla = 10;
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.EstadoPlanilla)
                  .WithErrorMessage("10 no puede ser de ese tipo de valor");
        }
        #endregion

        #region EstadoDocumento
        [Fact]
        public void EstadoDocumento_Debe_Fallar_Si_Esta_Vacio()
        {
            var command = CrearCommandValido();
            command.EstadoDocumento = null;
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.EstadoDocumento)
                  .WithErrorMessage("{estadoDocumento} no puede ser vacio o nulo");
        }

        [Fact]
        public void EstadoDocumento_Debe_Fallar_Si_No_Es_27()
        {
            var command = CrearCommandValido();
            command.EstadoDocumento = 10;
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.EstadoDocumento)
                  .WithErrorMessage("10 no puede ser de ese tipo de valor");
        }
        #endregion

        #region CodigoPerfilUsuario
        [Fact]
        public void CodigoPerfilUsuario_Debe_Fallar_Si_Vacio()
        {
            var command = CrearCommandValido();
            command.CodigoPerfilUsuario = null;
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.CodigoPerfilUsuario)
                  .WithErrorMessage("{codigoPerfilUsuario} no puede ser vacio o nulo");
        }

        [Fact]
        public void CodigoPerfilUsuario_Debe_Fallar_Si_Mayor_Que_99999()
        {
            var command = CrearCommandValido();
            command.CodigoPerfilUsuario = 100000;
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.CodigoPerfilUsuario)
                  .WithErrorMessage("{codigoPerfilUsuario} solo puede tener maximo 5 caracteres");
        }
        #endregion

        #region CodigoAgrupamiento
        [Fact]
        public void CodigoAgrupamiento_Deberia_Pasar_Si_FlagDesembolsoTotal_Y_Esta_Vacio()
        {
            var command = CrearCommandValido();
            command.FlagDesembolsoTotal = true;
            command.CodigoAgrupamiento = "";

            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(c => c.CodigoAgrupamiento);
        }

        [Fact]
        public void CodigoAgrupamiento_Deberia_Pasar_Si_No_FlagDesembolsoTotal_Y_Tiene_Codigo()
        {
            var command = CrearCommandValido();
            command.FlagDesembolsoTotal = false;
            command.CodigoAgrupamiento = "AGRUP1";

            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(c => c.CodigoAgrupamiento);
        }
        #endregion

        #region CodigoTienda
        [Fact]
        public void CodigoTienda_Debe_Fallar_Si_Vacio()
        {
            var command = CrearCommandValido();
            command.CodigoTienda = "";
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.CodigoTienda)
                  .WithErrorMessage("{codigoTienda} no puede ser vacio o nulo");
        }

        [Fact]
        public void CodigoTienda_Debe_Fallar_Si_Mayor_10_Caracteres()
        {
            var command = CrearCommandValido();
            command.CodigoTienda = "TIENDALARGA123";
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.CodigoTienda)
                  .WithErrorMessage("{codigoTienda} solo puede tener maximo 10 caracteres");
        }
        #endregion

        #region Comentario
        [Fact]
        public void Comentario_Debe_Fallar_Si_Vacio()
        {
            var command = CrearCommandValido();
            command.Comentario = "";
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.Comentario)
                  .WithErrorMessage("{comentario} no puede ser vacio o nulo");
        }

        [Fact]
        public void Comentario_Debe_Fallar_Si_Mayor_250_Caracteres()
        {
            var command = CrearCommandValido();
            command.Comentario = new string('a', 251);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.Comentario)
                  .WithErrorMessage("{comentario} solo puede tener maximo 250 caracteres");
        }
        #endregion

        #region NombreUsuarioRegistro
        [Fact]
        public void NombreUsuarioRegistro_Debe_Fallar_Si_Vacio()
        {
            var command = CrearCommandValido();
            command.NombreUsuarioRegistro = "";
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.NombreUsuarioRegistro)
                  .WithErrorMessage("{nombreUsuarioRegistro} no puede ser vacio o nulo");
        }

        [Fact]
        public void NombreUsuarioRegistro_Debe_Fallar_Si_Mayor_20_Caracteres()
        {
            var command = CrearCommandValido();
            command.NombreUsuarioRegistro = new string('a', 21);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.NombreUsuarioRegistro)
                  .WithErrorMessage("{nombreUsuarioRegistro} solo puede tener maximo 20 caracteres");
        }
        #endregion

        #region CanalAtencion
        [Fact]
        public void CanalAtencion_Debe_Fallar_Si_Vacio()
        {
            var command = CrearCommandValido();
            command.CanalAtencion = "";
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.CanalAtencion)
                  .WithErrorMessage("{canalAtencion} no puede ser vacio o nulo");
        }

        [Fact]
        public void CanalAtencion_Debe_Fallar_Si_Mayor_20_Caracteres()
        {
            var command = CrearCommandValido();
            command.CanalAtencion = new string('a', 21);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.CanalAtencion)
                  .WithErrorMessage("{canalAtencion} solo puede tener maximo 20 caracteres");
        }
        #endregion

        #region CodigoUsuario
        [Fact]
        public void CodigoUsuario_Debe_Fallar_Si_Vacio()
        {
            var command = CrearCommandValido();
            command.CodigoUsuario = "";
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.CodigoUsuario)
                  .WithErrorMessage("{codigoUsuario} no puede ser vacio o nulo");
        }

        [Fact]
        public void CodigoUsuario_Debe_Fallar_Si_Mayor_10_Caracteres()
        {
            var command = CrearCommandValido();
            command.CodigoUsuario = "USERLARGONAME";
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.CodigoUsuario)
                  .WithErrorMessage("{codigoUsuario} solo puede tener maximo 10 caracteres");
        }
        #endregion

        #region CodigoUnico
        [Fact]
        public void CodigoUnico_Debe_Fallar_Si_Vacio()
        {
            var command = CrearCommandValido();
            command.CodigoUnico = "";
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.CodigoUnico)
                  .WithErrorMessage("{codigoUnico} no puede ser vacio o nulo");
        }

        [Fact]
        public void CodigoUnico_Debe_Fallar_Si_No_Tiene_10_Digitos()
        {
            var command = CrearCommandValido();
            command.CodigoUnico = "12345";
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.CodigoUnico)
                  .WithErrorMessage("{codigoUnico} solo puede tener maximo 10 caracteres");
        }
        #endregion
    }
}
