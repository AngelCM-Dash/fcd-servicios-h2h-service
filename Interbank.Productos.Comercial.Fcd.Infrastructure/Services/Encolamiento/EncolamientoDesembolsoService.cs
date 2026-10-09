using AutoMapper;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.Encolamiento;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Encolamiento
{
    [ExcludeFromCodeCoverage]
    public class EncolamientoDesembolsoService : BackgroundService, IEncolamientoDesembolsoService
    {
        private readonly Channel<EncolarDesembolsoCommand> _channel;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EncolamientoDesembolsoService> _logger;
        private readonly IMapper _mapper;
        private int _enColaCount;

        public EncolamientoDesembolsoService(
            IServiceProvider serviceProvider,
            ILogger<EncolamientoDesembolsoService> logger,
            IMapper mapper)
        {
            _channel = Channel.CreateUnbounded<EncolarDesembolsoCommand>();
            _serviceProvider = serviceProvider;
            _logger = logger;
            _enColaCount = 0;
            _mapper = mapper;
        }

        public async Task EnqueueAsync(EncolarDesembolsoCommand desembolso)
        {
            await _channel.Writer.WriteAsync(desembolso);

            _logger.LogInformation(
                "Elemento encolado para el desembolso: {Planilla}",
                desembolso.NumeroPlanilla);

            IncrementarContador();
            LogEstadoCola();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var item in _channel.Reader.ReadAllAsync(stoppingToken))
            {
                await ProcesarItem(item, stoppingToken);
            }
        }


        private async Task ProcesarItem(EncolarDesembolsoCommand desembolsoCommand, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            using var scope = _serviceProvider.CreateScope();
            var desembolsoService = scope.ServiceProvider.GetRequiredService<IDesembolsoService>();

            try
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                _logger.LogInformation("Inicio Enqueue - ProcesoDesembolso para la planilla {Planilla} a las {Timestamp}", desembolsoCommand.NumeroPlanilla, timestamp);

                var dapperDesembolsarPlanilla = _mapper.Map<DesembolsoRequest>(desembolsoCommand);
                await desembolsoService.DesembolsarPlanilla(dapperDesembolsarPlanilla);
                timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

                _logger.LogInformation("Finalizo Enqueue - ProcesoDesembolso para la planilla {Planilla} a las {Timestamp}", desembolsoCommand.NumeroPlanilla, timestamp);
                DecrementarContador();
                LogEstadoCola();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Enqueue - ProcesoDesembolso para la planilla {Planilla}", desembolsoCommand.NumeroPlanilla);
            }
        }

        private void IncrementarContador()
        {
            _enColaCount++;
        }

        private void DecrementarContador()
        {
            _enColaCount--;
        }

        private void LogEstadoCola()
        {
            _logger.LogInformation("Elementos en cola Enqueue - ProcesoDesembolso: {EnColaCount}", _enColaCount);
        }
    }
}
