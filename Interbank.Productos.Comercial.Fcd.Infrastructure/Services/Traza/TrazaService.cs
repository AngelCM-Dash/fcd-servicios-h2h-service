using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Microsoft.Extensions.Logging;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Traza
{
    public class TrazaService : ITrazaService
    {
        private readonly ISeguimientoRepository _seguimientoRepository;
        private readonly ILogger<TrazaService> _logger;
        public TrazaService(ISeguimientoRepository seguimientoRepository, ILogger<TrazaService> logger)
        {
            ArgumentNullException.ThrowIfNull(seguimientoRepository);
            ArgumentNullException.ThrowIfNull(logger);

            _seguimientoRepository = seguimientoRepository;
            _logger = logger;
        }

        public Task<decimal> RegistrarCabeceraTraza(decimal IdDetalle, string? NombreArchivo, string? NroPlanilla, int Flag)
        {
            _logger.LogInformation("RegistrarCabeceraTraza llamado con IdDetalle: {IdDetalle}, NombreArchivo: {NombreArchivo}, NroPlanilla: {NroPlanilla}, Flag: {Flag}",
            IdDetalle, NombreArchivo, NroPlanilla, Flag);

            return _seguimientoRepository.InsertaSeguimientoCabecera(new DapperSeguimientoCabecera
            {
                IdDetalle = IdDetalle,
                NombreArchivo = NombreArchivo,
                NroPlanilla = NroPlanilla,
                Flag = Flag
            });
        }

        public Task<decimal> RegistrarDetalleTraza(decimal IdDetalle, string? NombreMetodo, string? CapaObservacion, string? DetalleObservacion, int IdEstacion)
        {
            _logger.LogInformation("RegistrarDetalleTraza llamado con IdDetalle: {IdDetalle}, NombreMetodo: {NombreMetodo}, CapaObservacion: {CapaObservacion}, DetalleObservacion: {DetalleObservacion}, IdEstacion: {IdEstacion}",
            IdDetalle, NombreMetodo, CapaObservacion, DetalleObservacion, IdEstacion);

            return _seguimientoRepository.InsertaSeguimientoDetalle(new DapperSeguimientoDetalle
            {
                IdDetalle = IdDetalle,
                NombreMetodo = NombreMetodo,
                CapaObservacion = CapaObservacion,
                DetalleObservacion = DetalleObservacion,
                IdEstacion = IdEstacion
            });
        }
    }
}
