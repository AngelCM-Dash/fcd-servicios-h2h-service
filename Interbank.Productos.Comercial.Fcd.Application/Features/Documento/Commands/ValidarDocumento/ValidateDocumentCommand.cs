using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response;
using MediatR;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Documento.Commands.ValidarDocumento
{
    public class ValidateDocumentCommand : IRequest<BaseResponse>
    {
        public string? CodigoUnico { get; set; }
        public int? CodigoProducto { get; set; }
        public string? NombreArchivo { get; set; }
        public int? Filtro { get; set; }

        public ValidateDocumentCommand(string? codigoUnico, int? codigoProducto, string? nombreArchivo, int? filtro)
        {
            CodigoUnico = codigoUnico;
            CodigoProducto = codigoProducto;
            NombreArchivo = nombreArchivo;
            Filtro = filtro;
        }
    }
}
