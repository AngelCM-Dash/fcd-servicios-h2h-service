using MediatR;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Common.Queries.GetTokenAuthorizationByChannel
{
    public class GetTokenAuthorizationByQuery : IRequest<TokenAuthorizationVM>
    {
        public string? Canal { get; set; }
        public int TipoScope { get; set; }

    }
}
