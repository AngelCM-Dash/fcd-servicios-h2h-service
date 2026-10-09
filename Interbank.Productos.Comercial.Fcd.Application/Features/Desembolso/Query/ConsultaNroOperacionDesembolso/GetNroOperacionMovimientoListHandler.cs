using AutoMapper;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using MediatR;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Query.ConsultaNroOperacionDesembolso
{
    public class GetNroOperacionMovimientoListHandler : IRequestHandler<GetNroOperacionMovimientoQuery, List<GetNroOperacionMovimientoQueryVM>>
    {
        private readonly IDesembolsoRepository _desembolsoRepository;
        private readonly IMapper _mapper;

        public GetNroOperacionMovimientoListHandler(IDesembolsoRepository desembolsoRepository, IMapper mapper)
        {
            _desembolsoRepository = desembolsoRepository;
            _mapper = mapper;
        }
        public async Task<List<GetNroOperacionMovimientoQueryVM>> Handle(GetNroOperacionMovimientoQuery request, CancellationToken cancellationToken)
        {
            var consultaMovimiento = await _desembolsoRepository.ObtenerNroOperacionDesembolso(request.NumeroPlanilla ?? string.Empty, request.NumeroSecuencia ?? string.Empty);

            if (consultaMovimiento == null || consultaMovimiento.Count == 0)
            {
                throw new NotFoundException("NO HAY INFORMACIÓN RELACIONADA A LA CONSULTA");
            }

            return _mapper.Map<List<GetNroOperacionMovimientoQueryVM>>(consultaMovimiento);

        }
    }
}
