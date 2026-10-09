namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper
{
    public class DapperActualizarPlanilla
    {
        public required string NumeroPlanilla { get; set; }
        public required int EstadoPlanilla { get; set; }
        public required int EstadoDocumento { get; set; }
        public required string? CodigoAgrupamiento { get; set; }
        public int CodigoPerfilUsuario { get; set; }
        public required string CodigoUsuario { get; set; }
        public required string NombreUsuarioRegistro { get; set; }
        public required string CanalAtencion { get; set; }
        public required string Comentario { get; set; }
        public required string CodigoTienda { get; set; }
        public int CodigoRetorno { get; set; }


    }
}
