using MediatR;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Excepciones.Queries
{
    public class GetCalculoInteresComisionQuery : IRequest<List<ComisionProveedorVM>>
    {
        public int codigoSecuencia { get; set; }

        public GetCalculoInteresComisionQuery(int CodigoSecuencia)
        {
            codigoSecuencia = CodigoSecuencia;

        }
    }
}
