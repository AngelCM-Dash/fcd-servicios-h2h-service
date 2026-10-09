using FluentValidation;
using System.Globalization;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Documento.Commands.RechazoDocumentos
{
    public class RejectionDocumentsValidator : AbstractValidator<RejectionDocumentsCommand>
    {
        public RejectionDocumentsValidator()
        {
            RuleFor(p => p.NumeroPlanilla)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{NumeroPlanilla} no puede ser vacio o nulo")
                .Length(10).WithMessage("{NumeroPlanilla} tiene que ser de 10 digitos")
                .When(p => p.FlagTipoRechazoDesembolso == 1 || p.FlagTipoRechazoDesembolso == 2);

            RuleFor(p => p.NumeroLinea).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{NumeroLinea} no puede ser vacio o nulo")
                .Length(8).WithMessage("{NumeroLinea} tiene que ser de 8 digitos")
                .When(p => p.FlagTipoRechazoDesembolso == 1);

            RuleFor(p => p.Observacion).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{Observacion} no puede ser vacio o nulo")
                .When(p => p.FlagTipoRechazoDesembolso == 1 || p.FlagTipoRechazoDesembolso == 2);

            RuleFor(p => p.FechaAdelanto).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{FechaAdelanto} no puede ser vacio o nulo")
                .Must(EsFechaValidaDDMMYYYY)
                .WithMessage("{FechaAdelanto} debe tener el formato DD/MM/YYYY")
                .When(p => p.FlagTipoRechazoDesembolso == 1);
        }

        private static bool EsFechaValidaDDMMYYYY(string? fecha)
        {
            if (string.IsNullOrWhiteSpace(fecha))
                return false;

            DateTime temp;
            return DateTime.TryParseExact(
                fecha,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out temp
            );
        }
    }
}
