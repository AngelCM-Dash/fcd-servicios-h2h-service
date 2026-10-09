using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;

namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence
{
    public interface ISeguimientoRepository
    {
        Task<decimal> InsertaSeguimientoDetalle(DapperSeguimientoDetalle command);
        Task<decimal> InsertaSeguimientoCabecera(DapperSeguimientoCabecera command);
        Task<int> ObtenerIdSeguimientoxPlanilla(string numeroPlanilla);
        Task<string> ObtenerTipoEjecucionSeguimientoxIdDetalle(decimal idDetalle);
        Task<List<DapperSeguimientoDetalleCabecera>> ObtenerSeguimientoDetalleCabecera(string? fechaDesde, string? fechaHasta, string? nombreArchivo, string? numeroPlanilla);
    }
}
