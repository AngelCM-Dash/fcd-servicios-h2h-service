using FluentValidation;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.DesembolsoPlanillasDiferidas
{
    public class DesembolsoPlanillasDiferidasValidator : AbstractValidator<DesembolsoPlanillasDiferidasCommand>
    {
        public DesembolsoPlanillasDiferidasValidator()
        {
            RuleFor(p => p.numeroPlanilla).Cascade(CascadeMode.Stop)
        .NotEmpty().WithMessage("{NumeroPlanilla} no puede ser vacio o nulo")
        .Length(10).WithMessage("{NumeroPlanilla} tiene que ser de 10 digitos");
        }
    }
}
