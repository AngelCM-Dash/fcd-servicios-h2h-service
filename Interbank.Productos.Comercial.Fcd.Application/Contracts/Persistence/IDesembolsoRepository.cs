using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using System.Data;

namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence
{
    public interface IDesembolsoRepository
    {
        Task ActualizarPlanillaDistribuido(DapperActualizarPlanilla actualizarPlanilla);
        Task ActualizarPlanillaDistribuidoFCD(DapperActualizarPlanilla actualizarPlanilla);
        Task<DataTable> ObtenerTramas(DapperGenerarTramasInput generarTramasInput);
        Task<string> fintNextPlanillaSecuencia(String NumeroPlanilla);
        Task<List<DapperParametro>> ObtenerFlujoDesembolso(string codigoReferencia, string descripcionCorta);
        Task<DapperRespuestaProcesoTramas> RegistrarTramasProcesadadas(string numeroPlanilla, int planillaSecuencia, string usuario);
        Task<DapperRespuestaProcesoTramas> RegistrarMovimientos(string numeroPlanilla, int planillaSecuencia, string usuario, string numeroInstruccion);
        Task<List<DapperConsumoLinea>> ObtenerDatosDesembolso(string numeroPlanilla, int numeroPlanillaSEQ, string tipoQuery);
        Task<List<DapperPlanillasDiferidas>> ObtenerPlanillasDiferidas(string tipoQuery);
        Task RechazarDocumentosDiferidos(string numeroPlanilla, string numeroInstruccion, string numeroLinea, string lineaObservacion);
        Task<DataTable> GenerarTramaDiferidos(string numeroPlanilla, string numeroInstruccion, string numeroLinea);
        Task<DataTable> ObtieneTramaDiferidos(string numeroPlanilla, int numeroPlanillaSecuencia);
        Task<int> RegistrarPlanillaDietario(string numeroPlanilla);
        Task RegistrarMensajeErrorMonitor(string numeroPlanilla, int numeroPlanillaSecuencia, string observacion);
        Task<int> ObtenerMensajeErrorMonitorPorPlanilla(string numeroPlanilla);
        Task RegistrarDesembolsoAbono(string numeroPlanilla, int numeroSecuencia, string codigoUnicoProveedor, int numeroReintento, int estadoReintento, int tipoReintento, int registroProcesoAbono);
        Task ActualizarEstadoDesembolsoAbono(string numeroPlanilla, int numeroSecuencia, string codigoUnicoProveedor, int estadoReintento, int registroProcesoAbono);
        Task<List<DapperTmpDesembolsoAbono>> ObtenerDesembolsoAbono(string numeroPlanilla, int numeroSecuencia, string codigoUnicoProveedor);
        Task<DataTable> ObtenerTramasParcialTotalPorProveedores(DapperGenerarTramasInput generarTramasInput, int tipoTramaParcial);
        Task<List<DapperNumeroOperacionDesembolso>> ObtenerNroOperacionDesembolso(string numeroPlanilla, string numeroSecuencia);


    }
}
