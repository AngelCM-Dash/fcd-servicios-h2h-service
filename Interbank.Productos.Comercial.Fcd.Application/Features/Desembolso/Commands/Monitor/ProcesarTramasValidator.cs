using FluentValidation;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.Monitor
{
    public class ProcesarTramasValidator : AbstractValidator<ProcesarTramasCommand>
    {
        public ProcesarTramasValidator()
        {
            RuleFor(p => p.FlagMonitor)
                       .Must(BeTrueOrFalse)
                       .WithMessage("El valor de {PropertyName} debe ser 'true' o 'false', no se permite otro valor.");

        }
        private static bool BeTrueOrFalse(bool? flag)
        {
            return flag.HasValue;
        }
    }
}
