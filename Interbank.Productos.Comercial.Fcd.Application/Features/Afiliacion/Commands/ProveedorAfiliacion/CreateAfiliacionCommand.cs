using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Afiliacion.Commands.ProveedorAfiliacion
{
    [ExcludeFromCodeCoverage]
    public class CreateAfiliacionCommand : IRequest<AfiliacionResponse>
    {
        public ClienteProveedorAfiliacion? Proveedor { get; set; }
        public int? DocumentoDuplicado { get; set; }
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
        public string? CodigoUnico { get; set; }
        public int EstadoProveedor { get; set; }
        public int? Tipomonedasoles { get; set; }
        public int? Tipomonedadolar { get; set; }
        public string? NumeroCtaSoles { get; set; }
        public string? NumeroCtaDolares { get; set; }
        public decimal TasaSoles { get; set; }
        public decimal TasaDolares { get; set; }
        public int? Portes { get; set; }
        public string? NumeroLineaCliente { get; set; }
        public string? NumeroLineaAceptante { get; set; }
        public decimal? MontoMinSolesAceptante { get; set; }
        public decimal? MontoMinDolaresAceptante { get; set; }
        public string? NumeroLineaProveedor { get; set; }
        public int? CodigoCliente { get; set; }
        public int? CodigoProducto { get; set; }
        public int? CodigoEstadoAfiliacion { get; set; }
        public int? TipoAfiliacion { get; set; }
        public string? UsuarioRegistro { get; set; }
        public string? RazonSocial { get; set; }
        public string? NumeroLinea { get; set; }
        public int? CodigoTipoDocumento { get; set; }
        public string? NumeroDocumento { get; set; }
        public string? DocumentoAuxiliarCliente { get; set; }
        public decimal? TasaClientesoles { get; set; }
        public decimal? TasaClienteDolar { get; set; }


        public int? ValidaCuentaSoles { get; set; }
        public int? ValidaCuentaDolares { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaUltimaActualizacion { get; set; }
        public int? DesembolsoAutoSoles { get; set; }
        public int? DesembolsoAutoDolar { get; set; }

    }
}
