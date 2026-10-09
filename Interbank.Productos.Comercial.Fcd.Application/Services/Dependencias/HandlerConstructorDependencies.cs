using AutoMapper;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions.IErrorHandling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Interbank.Productos.Comercial.Fcd.Application.Services.Dependencias
{
    public abstract class HandlerConstructorDependencies<T>
    {
        protected readonly IConfiguration _configuration;
        protected readonly IPlanillasRepository _planillaRepository;
        protected readonly IDesembolsoRepository _desembolsoRepository;
        protected readonly ISeguimientoRepository _seguimientoRepository;
        protected readonly IUtilitariosRepository _utilitariosRepository;
        protected readonly ICommonService _commonService;
        protected readonly IDesembolsoAbonoService _desembolsoService;
        protected readonly IMonitorService _monitorService;
        protected readonly ILineaService _lineaService;
        protected readonly IMapper _mapper;
        protected readonly IValidation _handlerValidation;
        protected readonly ILogger<T> _logger;
        protected readonly IEncrypterService _encrypter;

        protected HandlerConstructorDependencies(IServiceProvider serviceProvider)
        {
            _configuration = serviceProvider.GetRequiredService<IConfiguration>();
            _planillaRepository = serviceProvider.GetRequiredService<IPlanillasRepository>();
            _desembolsoRepository = serviceProvider.GetRequiredService<IDesembolsoRepository>();
            _seguimientoRepository = serviceProvider.GetRequiredService<ISeguimientoRepository>();
            _utilitariosRepository = serviceProvider.GetRequiredService<IUtilitariosRepository>();
            _commonService = serviceProvider.GetRequiredService<ICommonService>();
            _desembolsoService = serviceProvider.GetRequiredService<IDesembolsoAbonoService>();
            _monitorService = serviceProvider.GetRequiredService<IMonitorService>();
            _lineaService = serviceProvider.GetRequiredService<ILineaService>();
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _handlerValidation = serviceProvider.GetRequiredService<IValidation>();
            _logger = serviceProvider.GetRequiredService<ILogger<T>>();
            _encrypter = serviceProvider.GetRequiredService<IEncrypterService>();
        }
    }
}
