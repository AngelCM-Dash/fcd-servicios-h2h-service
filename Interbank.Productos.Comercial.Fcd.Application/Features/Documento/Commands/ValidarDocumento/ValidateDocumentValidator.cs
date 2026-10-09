using FluentValidation;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Documento.Commands.ValidarDocumento
{
    public class ValidateDocumentValidator : AbstractValidator<ValidateDocumentCommand>
    {
        public ValidateDocumentValidator()
        {
            RuleFor(p => p.CodigoUnico)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{CodigoUnico} no puede ser vacio o nulo")
                .Length(10).WithMessage("{CodigoUnico} tiene que ser de 10 digitos")
                .When(p => p.Filtro == 1 || p.Filtro == 2, ApplyConditionTo.CurrentValidator)
                .Unless(p => p.Filtro != 1 && p.Filtro != 2);

            RuleFor(p => p.CodigoProducto).Cascade(CascadeMode.Stop)
                .NotEqual(0).WithMessage("{CodigoProducto} no puede ser cero")
                .NotNull().WithMessage("Se debe enviar el valor del atributo {CodigoProducto}, no puede ser nulo")
                .Must(p => p == 31 || p == 34).WithMessage("{CodigoProducto} solo puede ser 31 o 34")
                .When(p => p.Filtro == 1 || p.Filtro == 2, ApplyConditionTo.CurrentValidator)
                .Unless(p => p.Filtro != 1);

            RuleFor(p => p.NombreArchivo).Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("{NombreArchivo} no puede ser vacio o nulo")
                    .When(p => p.Filtro == 1, ApplyConditionTo.CurrentValidator)
                    .Matches(@"^.*\.txt$").WithMessage("{NombreArchivo} debe tener la extensión .txt")
                    .Unless(p => p.Filtro != 1);

            RuleFor(p => p.Filtro).Cascade(CascadeMode.Stop)
                     .NotEmpty().WithMessage("{Filtro} no puede ser vacio o nulo")
                     .Must(codigo => codigo == 1 || codigo == 2)
                     .WithMessage("{Filtro} solo puede ser 1 para validar Documentos Duplicados o 2 para validar facturas pendientes a cargo");

        }
    }
}
