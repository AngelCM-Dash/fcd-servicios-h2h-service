namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper
{
    public class DapperAfiliacionProveedor
    {
        public int? documentoDuplicado { get; set; }
        public int? tipoMaxLote { get; set; }
        public int? tipoMaxProv { get; set; }
        public decimal? montoMaxLote { get; set; }
        public decimal? montoMaxProv { get; set; }
        public string? nombreContacto1 { get; set; }
        public string? emailContacto1 { get; set; }
        public string? cargoContacto1 { get; set; }
        public string? telefono1 { get; set; }
        public string? telefono2 { get; set; }
        public string? nombreContacto2 { get; set; }
        public string? emailContacto2 { get; set; }
        public string? nombreContacto3 { get; set; }
        public string? emailContacto3 { get; set; }
        public int? tipoComision { get; set; }
        public decimal? montoComision { get; set; }
        public int? ampliacionPago { get; set; }
        public string? codigoUnicoAceptante { get; set; }
        public int? estadoProveedor { get; set; }
        public int? tipomonedasoles { get; set; }
        public int? tipomonedadolar { get; set; }
        public string? numeroCtaSoles { get; set; }
        public string? numeroCtaDolares { get; set; }
        public decimal? tasaSoles { get; set; }
        public decimal? tasaDolares { get; set; }
        public int? portes { get; set; }
        public string? numeroLineaCliente { get; set; }
        public string? razonSocialAceptante { get; set; }
        public string? numeroLineaAceptante { get; set; }
        public decimal? montoMinSolesAceptante { get; set; }
        public decimal? montoMinDolaresAceptante { get; set; }
        public string? numeroLineaProveedor { get; set; }
        public int? codigoProveedor { get; set; }
        public int? codigoCliente { get; set; }
        public int? codigoAfiliacion { get; set; }
        public int? codigoProducto { get; set; }
        public int? codigoEstadoAfiliacion { get; set; }
        public int? tipoAfiliacion { get; set; }
        public string? usuarioRegistro { get; set; }
        public DateTime? fecharegistro { get; set; }
        public string? codigoUnico { get; set; }
        public string? razonSocial { get; set; }
        public string? numeroLinea { get; set; }
        public int? codigoTipoDocumento { get; set; }
        public string? numeroDocumento { get; set; }
        public string? documentoAuxiliarCliente { get; set; }
        public decimal? tasaClientesoles { get; set; }
        public decimal? tasaClienteDolar { get; set; }
        public int? validaCuentaSoles { get; set; }
        public int? validaCuentaDolares { get; set; }
        public DateTime? fechaIngreso { get; set; }
        public DateTime? fechaUltimaActualizacion { get; set; }
        public int? desembolsoAutoSoles { get; set; }
        public int? desembolsoAutoDolar { get; set; }
    }
}
