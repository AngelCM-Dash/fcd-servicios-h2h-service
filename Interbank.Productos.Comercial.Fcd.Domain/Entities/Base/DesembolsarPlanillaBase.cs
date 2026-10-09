namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Base
{
    public class DesembolsarPlanillaBase
    {
        public string? NumeroPlanilla { get; set; }
        public int? EstadoPlanilla { get; set; }
        public int? EstadoDocumento { get; set; }
        public string? CodigoAgrupamiento { get; set; }
        public int? CodigoPerfilUsuario { get; set; }
        public string? CodigoUsuario { get; set; }
        public string? NombreUsuarioRegistro { get; set; }
        public string? CanalAtencion { get; set; }
        public string? Comentario { get; set; }
        public string? CodigoTienda { get; set; }
        public Boolean FlagDesembolsoTotal { get; set; }
        public string? CodigoUnico { get; set; }
        public List<int>? CodigoReserva { get; set; }
    }
}
