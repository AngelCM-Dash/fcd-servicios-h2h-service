using FluentValidation.TestHelper;
using Interbank.Productos.Comercial.Fcd.Application.Features.Afiliacion.Commands.ProveedorAfiliacion;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Afiliacion.Commands.ProveedorAfiliacion
{
    public class CreateAfiliacionValidatorTest
    {
        private readonly CreateAfiliacionValidator _validator;

        public CreateAfiliacionValidatorTest()
        {
            _validator = new CreateAfiliacionValidator();
        }

        // Campos obligatorios que no deben ser nulos o vacíos
        [Theory]
        [InlineData("DocumentoDuplicado")]
        [InlineData("TipoMaxLote")]
        [InlineData("TipoMaxProv")]
        [InlineData("MontoMaxLote")]
        [InlineData("MontoMaxProv")]
        [InlineData("NombreContacto1")]
        [InlineData("EmailContacto1")]
        [InlineData("CodigoUnico")]
        [InlineData("EstadoProveedor")]
        [InlineData("TasaSoles")]
        [InlineData("TasaDolares")]
        [InlineData("Portes")]
        [InlineData("RazonSocial")]
        [InlineData("NumeroLineaAceptante")]
        [InlineData("CodigoCliente")]
        [InlineData("CodigoProducto")]
        [InlineData("CodigoEstadoAfiliacion")]
        public void Should_Have_Error_When_Required_Fields_Are_Null(string propertyName)
        {
            // Arrange
            var command = new CreateAfiliacionCommand(); // todo nulo por defecto

            // Act & Assert
            switch (propertyName)
            {
                case "DocumentoDuplicado":
                    _validator.TestValidate(command).ShouldHaveValidationErrorFor(c => c.DocumentoDuplicado);
                    break;
                case "TipoMaxLote":
                    _validator.TestValidate(command).ShouldHaveValidationErrorFor(c => c.TipoMaxLote);
                    break;
                case "TipoMaxProv":
                    _validator.TestValidate(command).ShouldHaveValidationErrorFor(c => c.TipoMaxProv);
                    break;
                case "MontoMaxLote":
                    _validator.TestValidate(command).ShouldHaveValidationErrorFor(c => c.MontoMaxLote);
                    break;
                case "MontoMaxProv":
                    _validator.TestValidate(command).ShouldHaveValidationErrorFor(c => c.MontoMaxProv);
                    break;
                case "NombreContacto1":
                    _validator.TestValidate(command).ShouldHaveValidationErrorFor(c => c.NombreContacto1);
                    break;
                case "EmailContacto1":
                    _validator.TestValidate(command).ShouldHaveValidationErrorFor(c => c.EmailContacto1);
                    break;
                case "CodigoUnico":
                    _validator.TestValidate(command).ShouldHaveValidationErrorFor(c => c.CodigoUnico);
                    break;
                case "EstadoProveedor":
                    _validator.TestValidate(command).ShouldHaveValidationErrorFor(c => c.EstadoProveedor);
                    break;
                case "TasaSoles":
                    _validator.TestValidate(command).ShouldHaveValidationErrorFor(c => c.TasaSoles);
                    break;
                case "TasaDolares":
                    _validator.TestValidate(command).ShouldHaveValidationErrorFor(c => c.TasaDolares);
                    break;
                case "Portes":
                    _validator.TestValidate(command).ShouldHaveValidationErrorFor(c => c.Portes);
                    break;
                case "RazonSocial":
                    _validator.TestValidate(command).ShouldHaveValidationErrorFor(c => c.RazonSocial);
                    break;
                case "NumeroLineaAceptante":
                    _validator.TestValidate(command).ShouldHaveValidationErrorFor(c => c.NumeroLineaAceptante);
                    break;
                case "CodigoCliente":
                    _validator.TestValidate(command).ShouldHaveValidationErrorFor(c => c.CodigoCliente);
                    break;
                case "CodigoProducto":
                    _validator.TestValidate(command).ShouldHaveValidationErrorFor(c => c.CodigoProducto);
                    break;
                case "CodigoEstadoAfiliacion":
                    _validator.TestValidate(command).ShouldHaveValidationErrorFor(c => c.CodigoEstadoAfiliacion);
                    break;
            }
        }

        [Fact]
        public void Should_Have_Error_When_CodigoProducto_Is_Invalid()
        {
            var command = new CreateAfiliacionCommand
            {
                CodigoProducto = 99 // no es 31 ni 34
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.CodigoProducto);
        }

        [Fact]
        public void Should_Have_Error_When_CodigoEstadoAfiliacion_Is_Invalid()
        {
            var command = new CreateAfiliacionCommand
            {
                CodigoEstadoAfiliacion = 999 // no es 201
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.CodigoEstadoAfiliacion);
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            var command = new CreateAfiliacionCommand
            {
                DocumentoDuplicado = 1,
                TipoMaxLote = 1,
                TipoMaxProv = 1,
                MontoMaxLote = 1000,
                MontoMaxProv = 500,
                NombreContacto1 = "Juan",
                EmailContacto1 = "juan@email.com",
                CodigoUnico = "C001",
                EstadoProveedor = 1,
                TasaSoles = 0.05m,
                TasaDolares = 0.04m,
                Portes = 10,
                RazonSocial = "Mi Empresa SAC",
                NumeroLineaAceptante = "123456",
                CodigoCliente = 1,
                CodigoProducto = 31,
                CodigoEstadoAfiliacion = 201
            };

            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
