namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Services
{
    public interface IFileService
    {
        string CopyFileOrDirectory(string sourcePath, string destinationFolder);
        string MoveFileOrDirectory(string sourcePath, string destinationFolder);
        string DeleteFileOrDirectory(string sourcePath);
    }
}
