namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response
{
    public class InstruccionAbonoResponse
    {
        public string? BusResponseCode { get; set; }
        public string? BusResponseMessage { get; set; }
        public string? SrvResponseCode { get; set; }
        public string? SrvResponseMessage { get; set; }
        public int? registrationNumberProcessed { get; set; }
        public int? returnedRegistrationNumber { get; set; }
    }

    public class InstruccionAbonoResponseBody
    {
        public int? RegistrationNumberProcessed { get; set; }
        public int? ReturnedRegistrationNumber { get; set; }
    }
}