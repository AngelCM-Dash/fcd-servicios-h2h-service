using FluentValidation;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Common
{
    public abstract class BasePlanillaValidator<T> : AbstractValidator<T> where T : IPlanillaCommand
    {
        protected BasePlanillaValidator()
        {
            RuleFor(p => p.CodigoProducto).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("{CodigoProducto} no puede ser nulo")
                .NotEqual(0).WithMessage("{CodigoProducto} no puede ser cero")
                .Must(p => p == 31 || p == 34)
                .WithMessage("Los códigos permitidos para {CodigoProducto} son 31 o 34");

            RuleFor(p => p.CanalAtencion)
                .NotEmpty().WithMessage("{CanalAtencion} no puede ser vacío o nulo");

            RuleFor(p => p.CodigoUsuario)
                .NotNull().WithMessage("{CodigoUsuario} no puede ser nulo")
                .NotEqual(0).WithMessage("{CodigoUsuario} no puede ser cero");

            RuleFor(p => p.Usuario)
                .NotEmpty().WithMessage("{Usuario} no puede ser vacío");

            RuleFor(p => p.CodigoPerfilUsuario).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("{CodigoPerfilUsuario} no puede ser nulo")
                .NotEqual(0).WithMessage("{CodigoPerfilUsuario} no puede ser cero");

            RuleFor(p => p.CodigoTienda).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("{CodigoTienda} no puede ser nulo")
                .NotEqual(0).WithMessage("{CodigoTienda} no puede ser cero");

            RuleFor(p => p.Tienda)
                .NotEmpty().WithMessage("{Tienda} no puede ser vacío o nulo");

            RuleFor(p => p.ContratoMarco)
                .NotNull().WithMessage("{ContratoMarco} no puede ser nulo");

            RuleFor(p => p.RutaArchivo)
                .NotEmpty().WithMessage("{RutaArchivo} no puede ser vacío o nulo");

            RuleFor(p => p.NombreArchivo).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{NombreArchivo} no puede ser vacío o nulo");

            RuleFor(p => p.CodigoUnico)
                .NotEmpty().WithMessage("{CodigoUnico} no puede ser vacío o nulo");

            RuleFor(p => p.FechaValor)
                .Must(p => p.HasValue && p.Value.Date >= DateTime.Today)
                .WithMessage("{FechaValor} debe ser mayor o igual a la fecha de hoy");
        }
    }
}
