namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper
{
    public class DapperComisionProveedor
    {
        public string? CodigoProveedor { get; set; }
        public decimal ProveedorImporteDescuento { get; set; }
        public decimal ProveedorImportePortes { get; set; }
        public decimal Total { get; set; }
    }
}

