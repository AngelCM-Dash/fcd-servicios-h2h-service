using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services;

namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp
{
    public interface ISftpService
    {
        Task Conectar(string host, int puerto, string usuario, string contraseña, string rutaClavePrivada, bool flagClave);
        Task<bool> ValidarRutaDirectorioRemotaSftp(string rutaRemota);
        Task<bool> ValidarArchivoExistenteSftp(string rutaRemota);
        Task<string[]> LeerLineasArchivoSftp(string rutaRemota);
        Task DescargarArchivoSftp(string rutaRemota, string rutaLocal);
        Task SubirArchivoSftp(Stream archivo, string rutaRemota);
        Task Desconectar();
        Task<bool> ValidarConexionSftp(SftpConfiguration config, int flag);
    }
}
