using Interbank.Productos.Comercial.Fcd.Api.Middleware;
using Interbank.Productos.Comercial.Fcd.Application;
using Interbank.Productos.Comercial.Fcd.Infrastructure;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Encrypter;
using Serilog;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var encrypter = new EncrypterService(configuration);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ApiResultFilter));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/*builder.Services.Configure<HostOptions>(op =>
{
    op.ServicesStartConcurrently = true;
    op.ServicesStopConcurrently = true;
});*/


builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration, encrypter);


builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

if (builder.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.UseMiddleware<DynatraceLogMiddleware>();

app.UseSerilogRequestLogging();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();
