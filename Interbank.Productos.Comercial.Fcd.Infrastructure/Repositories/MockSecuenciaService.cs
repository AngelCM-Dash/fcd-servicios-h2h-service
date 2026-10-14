using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Repositories
{
    // SOLO PARA PRUEBAS LOCALES. No utiliza Oracle ni agrega logs.
    // ../Pruebas/secuencias_mock.json es la única fuente de valores y estado.
    public static class MockSecuenciaService
    {
        private static readonly object Sync = new object();
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = false
        };

        private static string Archivo
        {
            get
            {
                var raizProyecto = EncontrarRaizProyecto();
                var carpetaH2H = Directory.GetParent(raizProyecto)?.FullName
                    ?? throw new DirectoryNotFoundException(
                        "No se encontró la carpeta H2H que contiene el proyecto.");

                return Path.Combine(carpetaH2H, "Pruebas", "secuencias_mock.json");
            }
        }

        private static string EncontrarRaizProyecto()
        {
            var actual = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (actual != null)
            {
                if (string.Equals(actual.Name, "fcd-servicios-h2h-service", StringComparison.OrdinalIgnoreCase))
                    return actual.FullName;

                var hijo = Path.Combine(actual.FullName, "fcd-servicios-h2h-service");
                if (Directory.Exists(Path.Combine(hijo, "docs")))
                    return hijo;

                actual = actual.Parent;
            }

            throw new DirectoryNotFoundException(
                "No se encontró la raíz fcd-servicios-h2h-service desde el directorio de trabajo actual.");
        }

        public static long Siguiente(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la secuencia es obligatorio.", nameof(nombre));

            lock (Sync)
            {
                string archivo = Archivo;
                // FileMode.Open: si falta el archivo, FALLA; nunca reinicia secuencias.
                FileStream? stream = null;
                for (int intento = 0; intento < 30; intento++)
                {
                    try
                    {
                        stream = new FileStream(archivo, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                        break;
                    }
                    catch (IOException) when (intento < 29 && File.Exists(archivo))
                    {
                        Thread.Sleep(100);
                    }
                }

                using (stream!)
                {
                    string contenido;
                    using (var lector = new StreamReader(stream!, Encoding.UTF8, true, 1024, leaveOpen: true))
                        contenido = lector.ReadToEnd();

                    if (string.IsNullOrWhiteSpace(contenido))
                        throw new InvalidDataException("El archivo de secuencias está vacío: " + archivo);

                    var valores = JsonSerializer.Deserialize<Dictionary<string, long>>(contenido, JsonOptions)
                        ?? throw new InvalidDataException("El archivo de secuencias no contiene un objeto JSON válido.");

                    if (!valores.TryGetValue(nombre, out long ultimo))
                        throw new KeyNotFoundException($"La secuencia '{nombre}' no está definida en {archivo}.");

                    long siguiente = checked(ultimo + 1);
                    valores[nombre] = siguiente;

                    byte[] bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(valores, JsonOptions));
                    stream!.Position = 0;
                    stream.Write(bytes, 0, bytes.Length);
                    stream.SetLength(bytes.Length);
                    stream.Flush(flushToDisk: true);
                    return siguiente;
                }
            }
        }
    }
}
