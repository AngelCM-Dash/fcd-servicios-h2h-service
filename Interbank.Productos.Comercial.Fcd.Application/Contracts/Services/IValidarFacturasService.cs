using Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.ValidarPlanilla;
using Interbank.Productos.Comercial.Fcd.Application.Models.Request;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using System.Data;

namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Services
{
    public interface IValidarFacturasService
    {
        SftpConnectionRequest CrearSftpConnectionRequest(List<DapperParametro> datosConfig, string nombreArchivo);
        DataTable InsertarProcesoDetallePlanilla(int numeroSecuencia, string[] contenido);
        Task<string> GenerarYSubirArchivoTxtAsync(List<ValidatePlanillaResponse> request, string nombreArchivo, SftpConnectionRequest sftpConfig, string remotePathUpload);
    }
}
