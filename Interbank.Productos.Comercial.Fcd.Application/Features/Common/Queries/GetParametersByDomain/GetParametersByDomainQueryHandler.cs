using AutoMapper;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using MediatR;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Common.Queries.GetParametersByDomain
{
    public class GetParametersByDomainQueryHandler : IRequestHandler<GetParametersByDomainQuery, List<ParametrosVM>>
    {
        private readonly IUtilitariosRepository _utilitariosRepository;
        private readonly IMapper _mapper;
        public GetParametersByDomainQueryHandler(IUtilitariosRepository utilitariosRepository, IMapper mapper)
        {
            _utilitariosRepository = utilitariosRepository;
            _mapper = mapper;
        }

        public async Task<List<ParametrosVM>> Handle(GetParametersByDomainQuery request, CancellationToken cancellationToken)
        {
            var dapperParametros = await _utilitariosRepository.ObtenerParametrosPorCodigoDominio(request.CodigoDominio);

            return _mapper.Map<List<ParametrosVM>>(dapperParametros);
        }
    }
}
