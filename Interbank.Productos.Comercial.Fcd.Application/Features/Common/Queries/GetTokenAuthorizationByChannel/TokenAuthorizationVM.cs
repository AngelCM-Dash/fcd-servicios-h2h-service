namespace Interbank.Productos.Comercial.Fcd.Application.Features.Common.Queries.GetTokenAuthorizationByChannel
{
    public class TokenAuthorizationVM
    {
        public string? token_type { get; set; }
        public string? access_token { get; set; }
        public string? scope { get; set; }
        public int? expires_in { get; set; }
        public int? consented_on { get; set; }
    }
}
