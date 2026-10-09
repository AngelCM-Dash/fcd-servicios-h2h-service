using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Data;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Excepciones.Commands
{
    public class CreateComisionProveedorHandler : IRequestHandler<CreateComisionProveedorCommand, int>
    {
        private readonly IExcepcionRepository _excepcionRepository;
        private readonly IUtilitariosRepository _utilitariosRepository;
        private readonly ISftpService _sftpService;
        private readonly IDataTableBuilderService _dataTableBuilderService;
        private readonly ILogger<CreateComisionProveedorHandler> _logger;

        public CreateComisionProveedorHandler(IExcepcionRepository excepcionRepository, IUtilitariosRepository utilitariosRepository, ISftpService sftpService, IDataTableBuilderService dataTableBuilderService, ILogger<CreateComisionProveedorHandler> logger)
        {
            _excepcionRepository = excepcionRepository;
            _utilitariosRepository = utilitariosRepository;
            _sftpService = sftpService;
            _dataTableBuilderService = dataTableBuilderService;
            _logger = logger;
        }

        public async Task<int> Handle(CreateComisionProveedorCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Inicio de Handle en CreateComisionProveedorHandler para archivo: {NombreArchivo}", request.nombreArchivo);

            List<DapperParametro> datosDominio = await _utilitariosRepository.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioConfiguracionSFTPCargaMasiva);
            if (datosDominio.Count == 0)
            {
                _logger.LogError("No se encontraron parámetros para el dominio SFTP (Dominio: {Dominio})", ParametroConstants.DominioConfiguracionSFTPCargaMasiva);
                throw new ThrowException("36", "Error al Obtener la información de la paramétrica de SFTP (ObtenerConfiguracionSFTP_H2H)");
            }

            try
            {
                var flag = Convert.ToInt32(datosDominio.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.FlagRed)?.DESCRIPCIONCORTA);
                string? password = datosDominio.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpPassword)?.DESCRIPCIONCORTA;
                var privateKeyLocalFilePath = datosDominio.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpRemotePathKey)?.DESCRIPCIONCORTA;

                string? Host = datosDominio.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpHost)?.DESCRIPCIONCORTA;
                int Port = Convert.ToInt32(datosDominio.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpPort)?.DESCRIPCIONCORTA);
                string? Username = datosDominio.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpUsername)?.DESCRIPCIONCORTA;
                string? RemotePath = datosDominio.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpRemotePath)?.DESCRIPCIONCORTA;

                _logger.LogDebug("Configuración SFTP obtenida. Host: {Host}, Puerto: {Port}, Usuario: {Username}, RutaRemota: {RemotePath}, Flag: {Flag}", Host, Port, Username, RemotePath, flag);

                await _sftpService.Conectar(Host ?? string.Empty, Port, Username ?? string.Empty, password ?? string.Empty, privateKeyLocalFilePath ?? string.Empty, Convert.ToBoolean(1));
                await _sftpService.ValidarRutaDirectorioRemotaSftp(RemotePath ?? string.Empty);
                await _sftpService.ValidarArchivoExistenteSftp(RemotePath + request.nombreArchivo);
                string[] contenido = await _sftpService.LeerLineasArchivoSftp(RemotePath + request.nombreArchivo);
                await _sftpService.Desconectar();
                _logger.LogInformation("Archivo leído correctamente desde SFTP");

                string contenidoArchivo = string.Join("\n", contenido);

                var codSecuencia = await _excepcionRepository.GeneraCodigoSecuenciaInteresComision();
                _logger.LogInformation("Código de secuencia generado: {CodigoSecuencia}", codSecuencia);

                JObject parametrosJson = JObject.Parse(contenidoArchivo);

                DataTable tablaParametros = _dataTableBuilderService.GeneraDatatableCalculoInteresComision(parametrosJson, codSecuencia);

                bool resultadoBulk = _utilitariosRepository.BulkInsertTable(tablaParametros, "TMP_CALCULO_COMISION_PROVEEDOR");
                if (!resultadoBulk)
                {
                    _logger.LogWarning("La inserción masiva de parámetros de comisión proveedor no fue exitosa para la secuencia: {CodigoSecuencia}", codSecuencia);
                }
                else
                {
                    _logger.LogInformation("Inserción masiva de parámetros de comisión proveedor exitosa para la secuencia: {CodigoSecuencia}", codSecuencia);
                }

                return codSecuencia;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en Handle de CreateComisionProveedorHandler para archivo: {NombreArchivo}", request.nombreArchivo);
                throw new ThrowException("36", ex.Message);

            }
        }
    }
}
