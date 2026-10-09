using AutoMapper;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices;
using MediatR;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Common.Queries.GetTokenAuthorizationByChannel
{
    public class GetTokenAuthorizationByChannelHandler : IRequestHandler<GetTokenAuthorizationByQuery, TokenAuthorizationVM>
    {
        private readonly ICommonService _commonService;
        private readonly IMapper _mapper;
        public GetTokenAuthorizationByChannelHandler(ICommonService commonService, IMapper mapper)
        {
            _commonService = commonService;
            _mapper = mapper;
        }
        public async Task<TokenAuthorizationVM> Handle(GetTokenAuthorizationByQuery request, CancellationToken cancellationToken)
        {
            var dapperParametros = await _commonService.GenerarTokenAuthorization(request.TipoScope, request.Canal ?? "");

            return _mapper.Map<TokenAuthorizationVM>(dapperParametros);
        }
    }
}
