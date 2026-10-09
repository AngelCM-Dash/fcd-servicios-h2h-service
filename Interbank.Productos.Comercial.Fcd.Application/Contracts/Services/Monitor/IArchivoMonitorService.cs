namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Monitor
{
    public interface IArchivoMonitorService
    {
        Task<string> GenerarArchivoPlano(string planilla, string? observacionCabecera, int flujo, int idCabeceraSeguimiento);
    }
}
