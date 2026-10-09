using AutoMapper;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persintence;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions.IErrorHandling;
using MediatR;
using static Interbank.Productos.Comercial.Fcd.Application.Constant.Constante;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Clientes.Queries.GetSuppliersList
{
    public class GetSuppliersListHandler : IRequestHandler<GetSuppliersListQuery, List<ClienteAfiliacionVM>>
    {
        private readonly IClienteAfiliacionRepository _clienteAfiliacionRepository;
        private readonly IMapper _mapper;
        private readonly IValidation _validation;

        public GetSuppliersListHandler(IClienteAfiliacionRepository clienteAfiliacionRepository, IMapper mapper, IValidation validation)
        {
            _clienteAfiliacionRepository = clienteAfiliacionRepository ?? throw new ArgumentNullException(nameof(clienteAfiliacionRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _validation = validation ?? throw new ArgumentNullException(nameof(validation));
        }

        public async Task<List<ClienteAfiliacionVM>> Handle(GetSuppliersListQuery request, CancellationToken cancellationToken)
        {

            if (!long.TryParse(request.CodigoUnicoAceptante.Trim(), out _))
            {
                _validation.AddValidationFailure(nameof(request.CodigoUnicoAceptante),
                    $"EL CÓDIGO ÚNICO '{request.CodigoUnicoAceptante}' NO TIENE UN FORMATO VÁLIDO.");
            }

            if (request.CodigoUnicoAceptante.Length > LongitudMaxCodigoUnico)
            {
                _validation.AddValidationFailure(nameof(request.CodigoUnicoAceptante),
                    "EL CÓDIGO ÚNICO ES MUY LARGA. SUPERA LA LONGITUD ESTANDAR");
            }

            int codigoProductoNumero = 0;

            if (request.CodigoProducto != null && !int.TryParse(request.CodigoProducto, out codigoProductoNumero))
            {
                _validation.AddValidationFailure(nameof(request.CodigoProducto),
                    $"EL CÓDIGO DE PRODUCTO '{request.CodigoProducto}' DEBE SER UN NÚMERO ENTERO.");
            }

            if (!Enum.IsDefined(typeof(Producto), codigoProductoNumero))
            {
                _validation.AddValidationFailure(nameof(request.CodigoProducto),
                    $"EL CÓDIGO DE PRODUCTO '{codigoProductoNumero}' NO EXISTE.");
            }

            _validation.ValidationExceptionIfThereAreErrors();

            int? codigoProducto = (int)Convert.ToInt64(request.CodigoProducto) == 0 ? null : (int)Convert.ToInt64(request.CodigoProducto);

            var proveedores = await _clienteAfiliacionRepository.GetSuppliersByAcceptor(request.CodigoUnicoAceptante, codigoProducto);

            if (!proveedores.Any())
            {
                throw new NotFoundException("NO HAY INFORMACIÓN RELACIONADA A LA CONSULTA");
            }

            return _mapper.Map<List<ClienteAfiliacionVM>>(proveedores);
        }
    }
}
