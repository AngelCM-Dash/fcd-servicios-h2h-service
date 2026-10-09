using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.OrdenarArchivosCtl
{
    public class OrdenarArchivosCtlCargaMasivaHandler : IRequestHandler<OrdenarArchivosCtlCargaMasivaCommand, string>
    {
        private readonly ILogger<OrdenarArchivosCtlCargaMasivaHandler> _logger;
        private readonly IConfiguration _configuration;

        public OrdenarArchivosCtlCargaMasivaHandler(ILogger<OrdenarArchivosCtlCargaMasivaHandler> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }


        public Task<string> Handle(OrdenarArchivosCtlCargaMasivaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string rutaDestinoBase = _configuration["pathCtlCargaMasiva"] ?? "";

                _logger.LogInformation("Iniciando proceso de ordenación y movimiento de archivos en la ruta: {Ruta}", rutaDestinoBase);

                var archivos = Directory.GetFiles(rutaDestinoBase)
                    .OrderBy(f => File.GetCreationTime(f).Year)
                    .ThenBy(f => File.GetCreationTime(f).Month)
                    .ThenBy(f => File.GetCreationTime(f).Day)
                    .ToArray();

                if (archivos.Length == 0)
                {
                    _logger.LogDebug("No hay archivos para mover en la ruta: {Ruta}", rutaDestinoBase);
                    return Task.FromResult("No hay archivos para mover.");
                }

                foreach (var archivo in archivos)
                {
                    DateTime fechaCreacion = File.GetCreationTime(archivo);

                    string año = fechaCreacion.ToString("yyyy");
                    string mes = fechaCreacion.ToString("MMMM", new System.Globalization.CultureInfo("es-ES"));
                    string dia = fechaCreacion.ToString("dd");

                    string rutaDestino = Path.Combine(rutaDestinoBase, año, mes, dia);

                    if (!Directory.Exists(rutaDestino))
                    {
                        Directory.CreateDirectory(rutaDestino);
                        _logger.LogDebug("Directorio creado: {Directorio}", rutaDestino);
                    }

                    string nombreArchivo = Path.GetFileName(archivo);

                    string destinoFinal = Path.Combine(rutaDestino, nombreArchivo);

                    _logger.LogDebug("Moviendo archivo {Archivo} a {Destino}", archivo, destinoFinal);

                    File.Move(archivo, destinoFinal, overwrite: true);
                }

                _logger.LogInformation("Archivos movidos correctamente.");
                return Task.FromResult("Archivos movidos correctamente.");

            }
            catch (Exception ex)
            {
                var ruta = _configuration["pathCtlCargaMasiva"];
                _logger.LogError(ex, "Error al mover archivos en la ruta: {Ruta}", ruta);
                throw new ThrowException("36", ex.Message);
            }

        }
    }
}
