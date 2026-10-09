namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence
{
    public interface IDocumentosRepository
    {
        Task RechazarDocumentosDiferidos(string numeroPlanilla, string numeroLinea, string lineaObservacion, string fechaAdelanto);
        Task RechazarDocumentosDistribuido(string numeroPlanilla, string observacion);
    }
}
