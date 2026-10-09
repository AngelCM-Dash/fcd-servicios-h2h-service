using Interbank.Productos.Comercial.Fcd.Application.Contracts.Abstractions;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persintence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Repositories;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Archivos;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Ctl;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Services.DataTableBuilder;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Desembolso;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Encolamiento;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Encrypter;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Lineas;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Notificacion;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Planilla;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Sftp;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Services.SqlLoader;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Traza;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Validaciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NextSIT.Utility;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure
{
    public static class InfraestuctureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, IEncrypterService encrypter)
        {
            var connectionString = configuration.GetConnectionString("connectionString") ?? "";
            var strCnnOra = encrypter.Decrypt(connectionString);

            services.AddScoped<IDapperExecutor, DapperExecutor>();
            services.AddTransient<IOracleConnectionFactory, OracleConnectionFactory>();
            services.AddTransient<IOracleBulkCopyExecutor, OracleBulkCopyExecutor>();

            services.AddDbContext<FcdDbContext>(options =>
                options.UseOracle(strCnnOra)
            );

            services.AddSingleton<ITypeConvertionManager, TypeConvertionManager>(e => TypeConvertionManager.GetNewTypeConvertionManager());

            services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
            //Registro de repositorios de infraestructura
            services.AddScoped<IClienteAfiliacionRepository, ClienteAfiliacionRepository>();
            services.AddScoped<IExcepcionRepository, ExcepcionRepository>();
            services.AddScoped<IPlanillasRepository, PlanillasRepository>();
            services.AddScoped<IDesembolsoRepository, DesembolsoRepository>();
            services.AddScoped<ISeguimientoRepository, SeguimientoRepository>();
            services.AddScoped<IUtilitariosRepository, UtilitariosRepository>();
            services.AddScoped<IAfiliacionRepository, AfiliacionRepository>();
            services.AddScoped<IDocumentosRepository, DocumentosRepository>();

            //Registro de servicios de infraestructura
            services.AddScoped<IDesembolsoService, DesembolsoService>();
            services.AddScoped<IMonitorService, MonitorService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<ILineaService, LineaService>();
            services.AddScoped<IPlanillaService, PlanillaService>();
            services.AddScoped<IEncrypterService, EncrypterService>();
            services.AddScoped<ISftpService, SftpService>();
            services.AddScoped<ISqlLoaderService, SqlLoaderService>();
            services.AddScoped<ICtlService, CtlService>();
            services.AddScoped<IDataTableBuilderService, DataTableBuilderService>();
            services.AddScoped<ITrazaService, TrazaService>();
            services.AddScoped<INotificacionService, NotificacionService>();
            services.AddSingleton<IEncolamientoPlanillaService, EncolamientoPlanillaService>();
            services.AddSingleton<IEncolamientoDesembolsoService, EncolamientoDesembolsoService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IValidarFacturasService, ValidarFacturasService>();


            services.AddSingleton<ISftpClientFactory, SftpClientFactory>();

            services.AddHostedService(provider => (EncolamientoPlanillaService)provider.GetRequiredService<IEncolamientoPlanillaService>());
            services.AddHostedService(provider => (EncolamientoDesembolsoService)provider.GetRequiredService<IEncolamientoDesembolsoService>());

            return services;
        }
    }
}