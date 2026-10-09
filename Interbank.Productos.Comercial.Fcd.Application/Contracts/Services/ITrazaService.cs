namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Services
{
    public interface ITrazaService
    {
        public Task<decimal> RegistrarCabeceraTraza(decimal IdDetalle, string? NombreArchivo, string? NroPlanilla, int Flag);
        public Task<decimal> RegistrarDetalleTraza(decimal IdDetalle, string? NombreMetodo, string? CapaObservacion, string? DetalleObservacion, int IdEstacion);
    }
}
