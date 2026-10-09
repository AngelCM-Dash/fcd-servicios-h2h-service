using MediatR;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.OrdenarArchivosCtl
{
    public class OrdenarArchivosCtlCargaMasivaCommand : IRequest<string>
    {
        public bool FlagOrdenarArchivos { get; set; }
    }
}
