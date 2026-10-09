using Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Common;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.CargaMasiva
{
    public class CreatePlanillasValidator : BasePlanillaValidator<CreatePlanillasCommand>
    {
        public CreatePlanillasValidator() : base()
        {
            // Aquí puedes agregar reglas adicionales solo para CreatePlanillasCommand
        }
    }
}
