using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;

namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence
{
    public interface IPlanillasRepository
    {
        Task<DapperPlanillaCompleta> RegistrarPlanillaFCD_H2H(DapperPlanillaCompleta planilla);
        Task<int> ObtenerCodigoFormacionFCD_H2H(DapperPlanillaCompleta planilla);
        Task RegistrarDocCuota_DocEstadoFCD_H2H(DapperPlanillaCompleta planilla);
        Task<DapperPlanillaCompleta> VerificarDuplicidadPlanillaFCD_H2H(DapperPlanillaCompleta planilla);
        Task<List<DapperDetallePlanillaMonitor>> ObtenerDetallePlanillaMonitor(string numeroPlanilla);
        Task<List<DapperReservasPlanilla>> ObtenerReservasPlanillas(string numeroPlanilla);
        Task<int> EliminarReservaPlanilla(string numeroPlanilla, int codigoReserva);
        Task<int> GeneraCodigoSecuenciaDocumentosDuplicados();
        Task<List<DapperDocumentosDuplicados>> ObtenerDocumentosDuplicados(string codigoUnico, int codigoSecuencia);
        Task<int> ValidarFacturaCargo(string codigoUnico);
        Task<int> ValidarPermiteDuplicados(string codigoUnico, int? codProducto);
        Task<DapperPlanillaCompleta> ObtenerInformacionPlanilla(string numeroPlanilla);
        Task<List<DapperPlanillaCompleta>> ObtenerInformacionDocumentoPlanilla(string numeroPlanilla);
        Task<List<DapperSecuenciaPlanilla>> ObtenerSecuenciaPlanilla(int cantidadSecuencias);
        Task<int> RegistraDocumentosPlanilla(string numeroPlanilla);
        Task RechazoPlanilla(string numeroPlanilla);
        Task<List<DapperParametroCtl>> ObtenerConfiguracionCTL(DapperPlanillaCompleta planilla);
        Task ActualizarObservacionPlanilla(string numeroPlanilla, string observacion);
    }

}
