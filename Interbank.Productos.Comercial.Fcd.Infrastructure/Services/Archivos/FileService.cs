using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Archivos
{
    public class FileService : IFileService
    {
        public string CopyFileOrDirectory(string sourcePath, string destinationFolder)
        {
            if (System.IO.File.Exists(sourcePath))
            {
                CopyFile(sourcePath, destinationFolder);
                return $"El archivo '{Path.GetFileName(sourcePath)}' se ha copiado exitosamente a '{destinationFolder}'.";
            }
            else if (Directory.Exists(sourcePath))
            {
                CopyDirectory(sourcePath, destinationFolder);
                return $"Todos los archivos de '{sourcePath}' se han copiado exitosamente a '{destinationFolder}'.";
            }
            else
            {
                throw new FileNotFoundException($"La ruta de origen '{sourcePath}' no existe.");
            }
        }

        public string MoveFileOrDirectory(string sourcePath, string destinationFolder)
        {
            if (System.IO.File.Exists(sourcePath))
            {
                MoveFile(sourcePath, destinationFolder);
                return $"El archivo '{Path.GetFileName(sourcePath)}' se ha movido exitosamente a '{destinationFolder}'.";
            }
            else if (Directory.Exists(sourcePath))
            {
                MoveDirectory(sourcePath, destinationFolder);
                return $"Todos los archivos de '{sourcePath}' se han movido exitosamente a '{destinationFolder}'.";
            }
            else
            {
                throw new FileNotFoundException($"La ruta de origen '{sourcePath}' no existe.");
            }
        }

        public string DeleteFileOrDirectory(string sourcePath)
        {
            if (System.IO.File.Exists(sourcePath))
            {
                System.IO.File.Delete(sourcePath);
                return $"El archivo '{sourcePath}' se ha eliminado exitosamente.";
            }
            else if (Directory.Exists(sourcePath))
            {
                foreach (var file in Directory.GetFiles(sourcePath))
                {
                    System.IO.File.Delete(file);
                }
                return $"Todos los archivos de '{sourcePath}' se han eliminado exitosamente.";
            }
            else
            {
                throw new FileNotFoundException($"La ruta de origen '{sourcePath}' no existe.");
            }
        }

        private static void CopyFile(string sourceFilePath, string destinationFolder)
        {
            Directory.CreateDirectory(destinationFolder);
            var destFilePath = Path.Combine(destinationFolder, Path.GetFileName(sourceFilePath));
            System.IO.File.Copy(sourceFilePath, destFilePath, overwrite: true);
        }

        private static void CopyDirectory(string sourceDir, string destinationFolder)
        {
            Directory.CreateDirectory(destinationFolder);
            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var destFile = Path.Combine(destinationFolder, Path.GetFileName(file));
                System.IO.File.Copy(file, destFile, overwrite: true);
            }
        }

        private static void MoveFile(string sourceFilePath, string destinationFolder)
        {
            Directory.CreateDirectory(destinationFolder);
            var destFilePath = Path.Combine(destinationFolder, Path.GetFileName(sourceFilePath));
            System.IO.File.Move(sourceFilePath, destFilePath, overwrite: true);
        }

        private static void MoveDirectory(string sourceDir, string destinationFolder)
        {
            Directory.CreateDirectory(destinationFolder);
            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var destFile = Path.Combine(destinationFolder, Path.GetFileName(file));
                System.IO.File.Move(file, destFile, overwrite: true);
            }
        }
    }
}
