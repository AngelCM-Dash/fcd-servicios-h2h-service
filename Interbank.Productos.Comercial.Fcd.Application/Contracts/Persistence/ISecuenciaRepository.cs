namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence
{
    public interface ISecuenciaRepository
    {
        Task<int> ConsultaSecuencia();
    }
}
