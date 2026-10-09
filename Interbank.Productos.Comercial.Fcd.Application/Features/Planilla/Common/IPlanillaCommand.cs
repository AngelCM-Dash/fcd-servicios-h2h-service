using Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.CargaMasiva;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Common
{
    public interface IPlanillaCommand
    {
        int? CodigoProducto { get; set; }
        string? CanalAtencion { get; set; }
        int? CodigoUsuario { get; set; }
        string? Usuario { get; set; }
        int? CodigoPerfilUsuario { get; set; }
        int? CodigoTienda { get; set; }
        string? Tienda { get; set; }
        bool? ContratoMarco { get; set; }
        string? RutaArchivo { get; set; }
        string? NombreArchivo { get; set; }
        string? CodigoUnico { get; set; }
        DateTime? FechaValor { get; set; }
        List<AdicionalRequest>? Adicional { get; set; }
    }
}
