namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Services
{
    public interface ISqlLoaderService
    {
        int ExecuteSQLLoader(string rutaArchivoData, string pathData, string ConfigPs);
    }
}
