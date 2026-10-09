using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.ActualizarPlanilla;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response;
using System.Data;

namespace Interbank.Productos.Comercial.Fcd.Application.Services
{
    public interface IDesembolsoAbonoService
    {
        Task ValidarEstadoProcesoAbono(ActualizarPlanillaCommand request, InstruccionAbonoResponse abono, int numeroIntento, int tipoReintentoAbono, int numeroPlanillaSeq, int idCabeceraSeguimiento, string codigoUnicoProveedor);
        Task InsertaDataFcdPagoMasiMaeBulk(InstruccionAbonoResponse abono, string numeroPlanilla, DapperParametroDesembolsoH2H parametrosDb2, DataTable dtTrama, int idCabeceraSeguimiento, int intento, int numeroPlanillaSeq);
    }
}
