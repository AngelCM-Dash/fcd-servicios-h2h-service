using Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Archivos;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.UnitTests.Services.Archivos
{
    public class FileServiceTest : IDisposable
    {
        private readonly string _tempDir;
        private readonly string _sourceFile;
        private readonly string _sourceDir;
        private readonly string _destinationDir;
        private readonly FileService _fileService;

        public FileServiceTest()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDir);

            // Archivo de prueba
            _sourceFile = Path.Combine(_tempDir, "testfile.txt");
            File.WriteAllText(_sourceFile, "Contenido de prueba");

            // Directorio de prueba
            _sourceDir = Path.Combine(_tempDir, "testdir");
            Directory.CreateDirectory(_sourceDir);
            File.WriteAllText(Path.Combine(_sourceDir, "file1.txt"), "Archivo 1");
            File.WriteAllText(Path.Combine(_sourceDir, "file2.txt"), "Archivo 2");

            // Destino
            _destinationDir = Path.Combine(_tempDir, "dest");

            _fileService = new FileService();
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, recursive: true);

            GC.SuppressFinalize(this);
        }

        // -----------------------------
        // CopyFileOrDirectory tests
        // -----------------------------
        [Fact]
        public void CopyFileOrDirectory_Should_Copy_File()
        {
            var result = _fileService.CopyFileOrDirectory(_sourceFile, _destinationDir);

            Assert.True(File.Exists(Path.Combine(_destinationDir, "testfile.txt")));
            Assert.Contains("se ha copiado exitosamente", result);
        }

        [Fact]
        public void CopyFileOrDirectory_Should_Copy_Directory()
        {
            var result = _fileService.CopyFileOrDirectory(_sourceDir, _destinationDir);

            Assert.True(File.Exists(Path.Combine(_destinationDir, "file1.txt")));
            Assert.True(File.Exists(Path.Combine(_destinationDir, "file2.txt")));
            Assert.Contains("se han copiado exitosamente", result);
        }

        [Fact]
        public void CopyFileOrDirectory_Should_Throw_When_Source_Not_Exist()
        {
            var nonExistentPath = Path.Combine(_tempDir, "noexist.txt");

            var ex = Assert.Throws<FileNotFoundException>(() =>
                _fileService.CopyFileOrDirectory(nonExistentPath, _destinationDir));

            Assert.Contains("no existe", ex.Message);
        }

        // -----------------------------
        // MoveFileOrDirectory tests
        // -----------------------------
        [Fact]
        public void MoveFileOrDirectory_Should_Move_File()
        {
            var result = _fileService.MoveFileOrDirectory(_sourceFile, _destinationDir);

            Assert.True(File.Exists(Path.Combine(_destinationDir, "testfile.txt")));
            Assert.False(File.Exists(_sourceFile));
            Assert.Contains("se ha movido exitosamente", result);
        }

        [Fact]
        public void MoveFileOrDirectory_Should_Move_Directory()
        {
            var result = _fileService.MoveFileOrDirectory(_sourceDir, _destinationDir);

            Assert.True(File.Exists(Path.Combine(_destinationDir, "file1.txt")));
            Assert.True(File.Exists(Path.Combine(_destinationDir, "file2.txt")));
            Assert.False(File.Exists(Path.Combine(_sourceDir, "file1.txt")));
            Assert.Contains("se han movido exitosamente", result);
        }

        [Fact]
        public void MoveFileOrDirectory_Should_Throw_When_Source_Not_Exist()
        {
            var nonExistentPath = Path.Combine(_tempDir, "noexist.txt");

            var ex = Assert.Throws<FileNotFoundException>(() =>
                _fileService.MoveFileOrDirectory(nonExistentPath, _destinationDir));

            Assert.Contains("no existe", ex.Message);
        }

        // -----------------------------
        // DeleteFileOrDirectory tests
        // -----------------------------
        [Fact]
        public void DeleteFileOrDirectory_Should_Delete_File()
        {
            var result = _fileService.DeleteFileOrDirectory(_sourceFile);

            Assert.False(File.Exists(_sourceFile));
            Assert.Contains("se ha eliminado exitosamente", result);
        }

        [Fact]
        public void DeleteFileOrDirectory_Should_Delete_Directory()
        {
            var result = _fileService.DeleteFileOrDirectory(_sourceDir);

            Assert.False(File.Exists(Path.Combine(_sourceDir, "file1.txt")));
            Assert.False(File.Exists(Path.Combine(_sourceDir, "file2.txt")));
            Assert.Contains("se han eliminado exitosamente", result);
        }

        [Fact]
        public void DeleteFileOrDirectory_Should_Throw_When_Source_Not_Exist()
        {
            var nonExistentPath = Path.Combine(_tempDir, "noexist.txt");

            var ex = Assert.Throws<FileNotFoundException>(() =>
                _fileService.DeleteFileOrDirectory(nonExistentPath));

            Assert.Contains("no existe", ex.Message);
        }
    }
}
