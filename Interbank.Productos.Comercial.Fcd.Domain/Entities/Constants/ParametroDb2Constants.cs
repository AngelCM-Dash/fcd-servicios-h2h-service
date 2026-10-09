namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants
{
    public class ParametroDb2Constants
    {
        public const int DominioMonitor = 1;
        public const int DominioDesembolso = 2;


        public enum NumOrdenDominioMonitor
        {
            PlanillasProcesadasDb2 = 1,
            DetallePlanillasProcesadasDb2 = 2,
            ActualizacionEstadoPlanillaDb2 = 3
        }
    }
}
