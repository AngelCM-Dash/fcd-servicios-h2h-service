namespace Interbank.Productos.Comercial.Fcd.Application.Features.Clientes.Queries.GetSuppliersList
{
    public class ClienteAfiliacionVM
    {
        public int docDuplicado { get; set; }
        public int tipoMaxLote { get; set; }
        public int tipoMaxProv { get; set; }
        public decimal montoMaxLote { get; set; }
        public decimal montoMaxProv { get; set; }
        public string? nombreContacto1 { get; set; }
        public string? emailContacto1 { get; set; }
        public string? cargoContacto1 { get; set; }
        public string? telefono1 { get; set; }
        public string? telefono2 { get; set; }
        public string? nombreContacto2 { get; set; }
        public string? emailContacto2 { get; set; }
        public string? nombreContacto3 { get; set; }
        public string? emailContacto3 { get; set; }
        public int tipoComision { get; set; }
        public decimal montoComision { get; set; }
        public int ampliacionPago { get; set; }
        public string? codigoUnicoAceptante { get; set; }
        public string? razonSocialAceptante { get; set; }
        public string? numeroLineaAceptante { get; set; }
        public decimal montoMinSolesAceptante { get; set; }
        public decimal montoMinDolarAceptante { get; set; }
        public int codigoProducto { get; set; }
        public int codigoEstadoAfiliacion { get; set; }
        public int tipoAfiliacion { get; set; }
        public DateTime fechaRegistro { get; set; }
        public string? codigoUsuarioRegistro { get; set; }
        public int estadoProveedor { get; set; }
        public string? codigoUnico { get; set; }
        public string? razonSocial { get; set; }
        public string? numeroLinea { get; set; }
        public int codigoTipoDocumento { get; set; }
        public string? numeroDocumento { get; set; }
        public string? documentoAuxiliarCliente { get; set; }
        public int validaCuentaSoles { get; set; }
        public int validaCuentaDolares { get; set; }
        public int desembolsoAutoSoles { get; set; }
        public int desembolsoAutoDolar { get; set; }
        public DateTime fechaIngreso { get; set; }
        public DateTime fechaAfiliacion { get; set; }
        public DateTime? fechaDesafiliacion { get; set; }
        public int tipoMonedaSoles { get; set; }
        public int tipoMonedaDolar { get; set; }
        public string? numeroCuentaSoles { get; set; }
        public string? numeroCuentaDolar { get; set; }
        public decimal tasaSoles { get; set; }
        public decimal tasaDolar { get; set; }
        public int portes { get; set; }
        public string? numeroLineaCliente { get; set; }
        public string? numeroLineaProveedor { get; set; }
        public int codigoCliente { get; set; }
        public int codigoProveedor { get; set; }
        public int codigoAfiliacion { get; set; }
        public DateTime fechaUltimaActualizacion { get; set; }
        public decimal tasaClienteSoles { get; set; }
        public decimal tasaClienteDolar { get; set; }
    }
}
