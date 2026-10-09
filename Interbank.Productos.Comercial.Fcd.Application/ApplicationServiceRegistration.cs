using FluentValidation;
using Interbank.Productos.Comercial.Fcd.Application.Behaviours;
using Interbank.Productos.Comercial.Fcd.Application.Common;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions.ErrorHandling;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions.IErrorHandling;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.ActualizarPlanilla;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.DesembolsoPlanillasDiferidas;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.Monitor;
using Interbank.Productos.Comercial.Fcd.Application.Features.Documento.Commands.ValidarDocumento;
using Interbank.Productos.Comercial.Fcd.Application.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Interbank.Productos.Comercial.Fcd.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddScoped<IValidation, Validation>();
            services.AddScoped<IDesembolsoAbonoService, DesembolsoAbonoService>();
            services.AddScoped(typeof(LoggingPipelineBehavior<,>));

            services.AddScoped<ActualizarPlanillaDependencies>(sp => new ActualizarPlanillaDependencies
            {
                PlanillasRepository = sp.GetRequiredService<IPlanillasRepository>(),
                DesembolsoRepository = sp.GetRequiredService<IDesembolsoRepository>(),
                UtilitariosRepository = sp.GetRequiredService<IUtilitariosRepository>(),
                SeguimientoRepository = sp.GetRequiredService<ISeguimientoRepository>(),
                DesembolsoService = sp.GetRequiredService<IDesembolsoService>(),
                DesembolsoAbonoService = sp.GetRequiredService<IDesembolsoAbonoService>(),
                NotificacionService = sp.GetRequiredService<INotificacionService>(),
                LineaService = sp.GetRequiredService<ILineaService>(),
            });

            services.AddScoped<ProcesarTramasDependencies>(sp => new ProcesarTramasDependencies
            {
                PlanillasRepository = sp.GetRequiredService<IPlanillasRepository>(),
                DesembolsoRepository = sp.GetRequiredService<IDesembolsoRepository>(),
                UtilitariosRepository = sp.GetRequiredService<IUtilitariosRepository>(),
                SeguimientoRepository = sp.GetRequiredService<ISeguimientoRepository>(),
                MonitorService = sp.GetRequiredService<IMonitorService>(),
                NotificacionService = sp.GetRequiredService<INotificacionService>(),
                LineaService = sp.GetRequiredService<ILineaService>(),
            });

            services.AddScoped<DesembolsoAbonoServiceDependencies>(sp => new DesembolsoAbonoServiceDependencies
            {
                DesembolsoRepository = sp.GetRequiredService<IDesembolsoRepository>(),
                UtilitariosRepository = sp.GetRequiredService<IUtilitariosRepository>(),
                MonitorService = sp.GetRequiredService<IMonitorService>(),
                DesembolsoService = sp.GetRequiredService<IDesembolsoService>(),
                NotificacionService = sp.GetRequiredService<INotificacionService>(),
            });

            services.AddScoped<DesembolsoPlanillasDiferidasDependencies>(sp => new DesembolsoPlanillasDiferidasDependencies
            {
                PlanillasRepository = sp.GetRequiredService<IPlanillasRepository>(),
                DesembolsoRepository = sp.GetRequiredService<IDesembolsoRepository>(),
                UtilitariosRepository = sp.GetRequiredService<IUtilitariosRepository>(),
                SeguimientoRepository = sp.GetRequiredService<ISeguimientoRepository>(),
                DesembolsoService = sp.GetRequiredService<IDesembolsoService>(),
                DesembolsoAbonoService = sp.GetRequiredService<IDesembolsoAbonoService>(),
                NotificacionService = sp.GetRequiredService<INotificacionService>(),
            });

            services.AddScoped<ValidateDocumentDependencies>(sp => new ValidateDocumentDependencies
            {
                PlanillasRepository = sp.GetRequiredService<IPlanillasRepository>(),
                UtilitariosRepository = sp.GetRequiredService<IUtilitariosRepository>(),
                TrazaService = sp.GetRequiredService<ITrazaService>(),
                NotificacionService = sp.GetRequiredService<INotificacionService>(),
                ValidarFacturasService = sp.GetRequiredService<IValidarFacturasService>(),
                SftpService = sp.GetRequiredService<ISftpService>(),
            });

            return services;

        }
    }
}
