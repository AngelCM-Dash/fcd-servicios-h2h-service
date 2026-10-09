using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Documento.Commands.ValidarDocumento
{
    public class ValidateDocumentDependencies
    {
        //Repositorios
        public IUtilitariosRepository UtilitariosRepository { get; init; } = null!;
        public IPlanillasRepository PlanillasRepository { get; init; } = null!;

        //Services
        public ITrazaService TrazaService { get; init; } = null!;
        public INotificacionService NotificacionService { get; init; } = null!;
        public IValidarFacturasService ValidarFacturasService { get; init; } = null!;
        public ISftpService SftpService { get; init; } = null!;
    }
}
