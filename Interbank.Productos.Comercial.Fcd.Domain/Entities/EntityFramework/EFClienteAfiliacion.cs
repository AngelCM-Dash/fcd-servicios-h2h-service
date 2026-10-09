namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.EntityFramework
{
    public class EFClienteAfiliacion
    {
        public EFClienteAfiliacion()
        {

        }

        public int? DocDuplicado { get; set; }
        public int? TipoMaxLote { get; set; }
        public int? TipoMaxProv { get; set; }
        public decimal? MontoMaxLote { get; set; }
        public decimal? MontoMaxProv { get; set; }

        public string? NombreContacto1 { get; set; }
        public string? EmailContacto1 { get; set; }
        public string? CargoContacto1 { get; set; }
        public string? Telefono1 { get; set; }
        public string? Telefono2 { get; set; }

        public string? NombreContacto2 { get; set; }
        public string? EmailContacto2 { get; set; }

        public string? NombreContacto3 { get; set; }
        public string? EmailContacto3 { get; set; }

        public int? TipoComision { get; set; }
        public decimal? MontoComision { get; set; }
        public int? AmpliacionPago { get; set; }

        public string? CodigoUnicoAceptante { get; set; }
        public string? RazonSocialAceptante { get; set; }
        public string? NumeroLineaAceptante { get; set; }
        public decimal? MontoMinSolesAceptante { get; set; }
        public decimal? MontoMinDolarAceptante { get; set; }

        public int? CodigoCliente { get; set; }
        public int? CodigoProveedor { get; set; }
        public int? CodigoAfiliacion { get; set; }
        public int? CodigoProducto { get; set; }
        public int? CodigoEstadoAfiliacion { get; set; }
        public int? TipoAfiliacion { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public string? CodigoUsuarioRegistro { get; set; }
        public DateTime? FechaRegistro { get; set; }

        public int? EstadoProveedor { get; set; }
        public string? CodigoUnico { get; set; }
        public string? RazonSocial { get; set; }
        public string? NumeroLinea { get; set; }
        public int? CodigoTipoDocumento { get; set; }
        public string? NumeroDocumento { get; set; }
        public string? DocumentoAuxiliarCliente { get; set; }

        public int? TipoMonedaSoles { get; set; }
        public int? TipoMonedaDolar { get; set; }
        public string? NumeroCuentaSoles { get; set; }
        public string? NumeroCuentaDolar { get; set; }
        public decimal? TasaSoles { get; set; }
        public decimal? TasaDolar { get; set; }
        public int? ValidaCuentaSoles { get; set; }
        public int? ValidaCuentaDolares { get; set; }
        public int? DesembolsoAutoSoles { get; set; }
        public int? DesembolsoAutoDolar { get; set; }
        public int? Portes { get; set; }

        public string? NumeroLineaCliente { get; set; }
        public string? NumeroLineaProveedor { get; set; }
        public DateTime? FechaUltimaActualizacion { get; set; }
        public DateTime? FechaAfiliacion { get; set; }
        public DateTime? FechaDesafiliacion { get; set; }

        public decimal? TasaClienteSoles { get; set; }
        public decimal? TasaClienteDolar { get; set; }
    }
}
