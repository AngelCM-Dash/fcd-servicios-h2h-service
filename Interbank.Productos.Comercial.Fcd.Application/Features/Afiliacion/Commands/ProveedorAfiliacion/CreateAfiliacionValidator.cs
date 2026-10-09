using FluentValidation;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Afiliacion.Commands.ProveedorAfiliacion
{
    public class CreateAfiliacionValidator : AbstractValidator<CreateAfiliacionCommand>
    {
        public CreateAfiliacionValidator()
        {
            RuleFor(p => p.DocumentoDuplicado)
                .NotEmpty().WithMessage("{DocumentoDuplicado} no puede ser vacio o nulo");

            RuleFor(p => p.TipoMaxLote)
                .NotEmpty().WithMessage("{TipoMaxLote} no puede ser vacio o nulo");

            RuleFor(p => p.TipoMaxProv)
                .NotEmpty().WithMessage("{TipoMaxProv} no puede ser vacio o nulo");

            RuleFor(p => p.MontoMaxLote)
                .NotEmpty().WithMessage("{MontoMaxLote} no puede ser vacio o nulo");

            RuleFor(p => p.MontoMaxProv)
                .NotEmpty().WithMessage("{MontoMaxProv} no puede ser vacio o nulo");

            RuleFor(p => p.NombreContacto1)
                .NotEmpty().WithMessage("{NombreContacto1} no puede ser vacio o nulo")
                .NotNull().WithMessage("Se debe enviar el valor del atributo {NombreContacto1}, no puede ser nulo");

            RuleFor(p => p.EmailContacto1)
                .NotEmpty().WithMessage("{EmailContacto1} no puede ser vacio o nulo")
                .NotNull().WithMessage("Se debe enviar el valor del atributo {EmailContacto1}, no puede ser nulo");

            RuleFor(p => p.CodigoUnico)
               .NotEmpty().WithMessage("{CodigoUnico} no puede ser vacio o nulo")
               .NotNull().WithMessage("Se debe enviar el valor del atributo {CodigoUnico}, no puede ser nulo");

            RuleFor(p => p.EstadoProveedor)
               .NotEmpty().WithMessage("{EstadoProveedor} no puede ser vacio o nulo")
               .NotNull().WithMessage("Se debe enviar el valor del atributo {EstadoProveedor}, no puede ser nulo");

            RuleFor(p => p.TasaSoles)
               .NotEmpty().WithMessage("{TasaSoles} no puede ser vacio o nulo");

            RuleFor(p => p.TasaDolares)
               .NotEmpty().WithMessage("{TasaDolares} no puede ser vacio o nulo");

            RuleFor(p => p.Portes)
               .NotEmpty().WithMessage("{Portes} no puede ser vacio o nulo");

            RuleFor(p => p.RazonSocial)
               .NotEmpty().WithMessage("{RazonSocial} no puede ser vacio o nulo")
               .NotNull().WithMessage("Se debe enviar el valor del atributo {RazonSocial}, no puede ser nulo");

            RuleFor(p => p.NumeroLineaAceptante)
               .NotEmpty().WithMessage("{NumeroLineaAceptante} no puede ser vacio o nulo")
               .NotNull().WithMessage("Se debe enviar el valor del atributo {NumeroLineaAceptante}, no puede ser nulo");

            RuleFor(p => p.CodigoCliente)
               .NotEmpty().WithMessage("{CodigoCliente} no puede ser vacio o nulo")
               .NotNull().WithMessage("Se debe enviar el valor del atributo {CodigoCliente}, no puede ser nulo");

            RuleFor(p => p.CodigoProducto).Cascade(CascadeMode.Stop)
               .NotEqual(0).WithMessage("{CodigoProducto} no puede ser cero")
               .NotNull().WithMessage("Se debe enviar el valor del atributo {CodigoProducto}, no puede ser nulo")
               .Must(p => p == 31 || p == 34).WithMessage("Los codigos permitidos para {CodigoProducto} son 31 o 34, verificar");

            RuleFor(p => p.CodigoEstadoAfiliacion).Cascade(CascadeMode.Stop)
               .NotEqual(0).WithMessage("{CodigoEstadoAfiliacion} no puede ser cero")
               .NotNull().WithMessage("Se debe enviar el valor del atributo {CodigoEstadoAfiliacion}, no puede ser nulo")
               .Must(p => p == 201).WithMessage("Los codigos permitidos para {CodigoEstadoAfiliacion} son 31 o 34, verificar");
        }
    }
}
