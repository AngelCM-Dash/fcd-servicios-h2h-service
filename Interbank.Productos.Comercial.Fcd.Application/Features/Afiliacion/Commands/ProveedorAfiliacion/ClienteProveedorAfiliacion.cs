namespace Interbank.Productos.Comercial.Fcd.Application.Features.Afiliacion.Commands.ProveedorAfiliacion
{
    public class ClienteProveedorAfiliacion
    {
        public string? CodigoUnico { get; set; }
        public int CodigoCliente { get; set; }
        public int Codigotipodocumento { get; set; }
        public string? NumeroDocumento { get; set; }
        public string? Segmento { get; set; }
        public string? CodigoEjecutivo { get; set; }
        public string? NombreEjecutivo { get; set; }
        public string? RazonSocial { get; set; }
        public int? CodigoTipoCliente { get; set; }
        public string? banca { get; set; }
        public decimal RatingEmpresa { get; set; }
        public string? IdCiiu { get; set; }
        public string? CodigoTienda { get; set; }
        public string? NombreTienda { get; set; }
        public string? ClasificacionSbs { get; set; }
        public string? ClasificacionFeve { get; set; }
        public string? CodigoGrupo { get; set; }
        public string? NombreGrupo { get; set; }
        public string? IdDireccion { get; set; }
        public string? IdDistrito { get; set; }
        public string? IdProvincia { get; set; }
        public string? IdDepartamento { get; set; }
    }
}
