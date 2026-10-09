using Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.CargaMasiva;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Common
{
    public abstract class PlanillaCommandBase : IPlanillaCommand
    {
        public int? CodigoProducto { get; set; }
        public string? CanalAtencion { get; set; }
        public int? CodigoUsuario { get; set; }
        public string? Usuario { get; set; }
        public int? CodigoPerfilUsuario { get; set; }
        public int? CodigoTienda { get; set; }
        public string? Tienda { get; set; }
        public bool? ContratoMarco { get; set; }
        public string? RutaArchivo { get; set; }
        public string? NombreArchivo { get; set; }
        public string? CodigoUnico { get; set; }
        public DateTime? FechaValor { get; set; }
        public List<AdicionalRequest>? Adicional { get; set; }
    }

}
