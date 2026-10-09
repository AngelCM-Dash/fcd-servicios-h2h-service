using AutoMapper;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using MediatR;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Excepciones.Queries
{
    public class GetCalculoInteresComisionListHandler : IRequestHandler<GetCalculoInteresComisionQuery, List<ComisionProveedorVM>>
    {
        private readonly IExcepcionRepository _excepcionRepository;
        private readonly IMapper _mapper;

        public GetCalculoInteresComisionListHandler(IExcepcionRepository excepcionRepository, IMapper mapper)
        {
            _excepcionRepository = excepcionRepository;
            _mapper = mapper;
        }

        public async Task<List<ComisionProveedorVM>> Handle(GetCalculoInteresComisionQuery request, CancellationToken cancellationToken)
        {
            var calculoComision = await _excepcionRepository.ConsultaCalculoInteresComision(request.codigoSecuencia);


            if (calculoComision == null || calculoComision.Count == 0)
            {
                throw new NotFoundException("NO HAY INFORMACIÓN RELACIONADA A LA CONSULTA");
            }

            return _mapper.Map<List<ComisionProveedorVM>>(calculoComision);

        }
    }
}
