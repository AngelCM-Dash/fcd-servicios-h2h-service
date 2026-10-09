using MediatR;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Common.Queries.GetParametersByDomain
{
    public class GetParametersByDomainQuery : IRequest<List<ParametrosVM>>
    {
        public int CodigoDominio { get; set; }
    }
}
