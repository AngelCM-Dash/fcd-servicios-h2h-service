using AutoMapper;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions.IErrorHandling;
using MediatR;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Seguimiento.Queries.GetListTrackingDetailHeader
{
    public class GetListTrackingDetailHeaderHandler : IRequestHandler<GetListTrackingDetailQuery, List<DetalleCabeceraVM>>
    {
        private readonly ISeguimientoRepository _seguimientoRepository;
        private readonly IMapper _mapper;
        private readonly IValidation _validation;

        public GetListTrackingDetailHeaderHandler(ISeguimientoRepository seguimientoRepository, IMapper mapper, IValidation validation)
        {
            _seguimientoRepository = seguimientoRepository ?? throw new ArgumentNullException(nameof(seguimientoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _validation = validation ?? throw new ArgumentNullException(nameof(validation));
        }

        public async Task<List<DetalleCabeceraVM>> Handle(GetListTrackingDetailQuery request, CancellationToken cancellationToken)
        {
            _validation.ValidationExceptionIfThereAreErrors();


            var proveedores = await _seguimientoRepository.ObtenerSeguimientoDetalleCabecera(request.fechaDesde, request.fechaHasta, request.nombreArchivo, request.numeroPlanilla);

            if (proveedores == null || proveedores.Count == 0)
            {
                throw new NotFoundException("NO HAY INFORMACIÓN RELACIONADA A LA CONSULTA");
            }

            return _mapper.Map<List<DetalleCabeceraVM>>(proveedores);
        }
    }
}
