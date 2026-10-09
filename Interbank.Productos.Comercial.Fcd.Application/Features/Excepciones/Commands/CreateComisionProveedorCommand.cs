using MediatR;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Excepciones.Commands
{
    public class CreateComisionProveedorCommand : IRequest<int>
    {
        public string nombreArchivo { get; set; }

        public CreateComisionProveedorCommand(string NombreArchivo)
        {
            nombreArchivo = NombreArchivo;
        }
    }
}
