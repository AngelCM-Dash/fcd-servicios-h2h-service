using Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.CargaMasiva;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;

namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices
{
    public interface IPlanillaService
    {
        Task<PlanillaResponse> CargaMasivaPlanilla(CargaMasivaRequest planillaRequest);
        Task<PlanillaResponse> EncolamientoCargaMasiva(CargaMasivaRequest planillaRequest);
    }
}
