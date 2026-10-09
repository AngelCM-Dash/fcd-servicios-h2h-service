namespace Interbank.Productos.Comercial.Fcd.Application.Features.Excepciones.Queries
{
    public class ComisionProveedorVM
    {
        public string? codigoProveedor { get; set; }
        public decimal proveedorImporteDescuento { get; set; }
        public decimal proveedorImportePortes { get; set; }
        public decimal total { get; set; }
    }
}
