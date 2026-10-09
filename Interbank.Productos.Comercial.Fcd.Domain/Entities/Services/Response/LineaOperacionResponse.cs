using Newtonsoft.Json;

namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response
{
    public class LineaOperacionResponse
    {
        [JsonProperty("creditLines")]
        public List<LineasCredito>? CreditLines { get; set; }
    }

    public class LineasCredito
    {

        [JsonProperty("creditLine")]
        public string? NumeroLinea { get; set; }

        [JsonProperty("operationType")]
        public string? CodigoLineaOperacion { get; set; }

        [JsonProperty("productId")]
        public int CodigoProducto { get; set; }

        [JsonProperty("productName")]
        public string? DescripcionProducto { get; set; }

        [JsonProperty("ibkProductId")]
        public string? CodigoIbProducto { get; set; }

        [JsonProperty("Specialized")]
        public int Especializado { get; set; }

        [JsonProperty("customercode")]
        public int CodigoCliente { get; set; }

        [JsonProperty("customerId")]
        public string? CodigoUnico { get; set; }

        [JsonProperty("customerEconomicGroupId")]
        public int CodigoGrupoEconomico { get; set; }

        [JsonProperty("branchCode")]
        public string? CodigoTienda { get; set; }

        [JsonProperty("branchName")]
        public string? NombreTienda { get; set; }

        [JsonProperty("riskMaximumClient")]
        public decimal RiesgoMaximoCliente { get; set; }

        [JsonProperty("riskMaximumGroup")]
        public decimal RiesgoMaximoGrupo { get; set; }

        [JsonProperty("creditTypeCode")]
        public int CodigoTipoCredito { get; set; }

        [JsonProperty("creditTypeDescription")]
        public string? DescripcionTipoCredito { get; set; }

        [JsonProperty("codeOperationType")]
        public int CodigoTipoOperacion { get; set; }

        [JsonProperty("operationTypeDescription")]
        public string? DescripcionTipoOperacion { get; set; }

        [JsonProperty("engaged")]
        public string? Comprometida { get; set; }

        [JsonProperty("approvalDate")]
        public DateTime FechaAprobacion { get; set; }

        [JsonProperty("dueDate")]
        public DateTime FechaVencimiento { get; set; }

        [JsonProperty("statusCode")]
        public int CodigoEstado { get; set; }

        [JsonProperty("statusDescription")]
        public string? DescripcionEstado { get; set; }

        [JsonProperty("approvedAmount")]
        public decimal MontoAprobado { get; set; }

        [JsonProperty("usedAmount")]
        public decimal MontoUtilizado { get; set; }

        [JsonProperty("amountAvailable")]
        public decimal MontoDisponible { get; set; }

        [JsonProperty("amountAssigned")]
        public int MontoCedido { get; set; }

        [JsonProperty("balanceBeforeOperation")]
        public decimal SaldoAntesOperacion { get; set; }

        [JsonProperty("reservedBalance")]
        public decimal SaldoReservado { get; set; }

        [JsonProperty("receivedAmount")]
        public int MontoRecibido { get; set; }

        [JsonProperty("amortizationNumber")]
        public int NumeroAmortizacion { get; set; }

        [JsonProperty("renewalnumber")]
        public int NumeroRenovacion { get; set; }

        [JsonProperty("fixedFee")]
        public int CuotaFija { get; set; }

        [JsonProperty("validityLetterCredit")]
        public int ValidezCartaCredito { get; set; }

        [JsonProperty("termOperation")]
        public string? PlazoOperacion { get; set; }

        [JsonProperty("scheduleSpecial")]
        public string? CronogramaEspecial { get; set; }

        [JsonProperty("specialCommission")]
        public decimal ComisionEspecial { get; set; }

        [JsonProperty("codeProject")]
        public string? CodigoProyecto { get; set; }

        [JsonProperty("nameProject")]
        public string? NombreProyecto { get; set; }

        [JsonProperty("frequencyPayment")]
        public int FrecuenciaPago { get; set; }

        [JsonProperty("descriptionFrequencyPayment")]
        public string? DescripcionFrecuenciaPago { get; set; }

        [JsonProperty("firstExpiration")]
        public string? PrimerVencimiento { get; set; }

        [JsonProperty("disbursementTerm")]
        public string? PlazoDesembolso { get; set; }

        [JsonProperty("codeCurrency")]
        public int CodigoMoneda { get; set; }

        [JsonProperty("descriptionCurrency")]
        public string? DescripcionMoneda { get; set; }

        [JsonProperty("amountFee")]
        public decimal MontoCuota { get; set; }

        [JsonProperty("amortizationPercentage")]
        public decimal PorcentajeAmortizacion { get; set; }

        [JsonProperty("codeProposal")]
        public string? CodigoPropuesta { get; set; }

        [JsonProperty("codeLineCredit")]
        public string? CodigoLineaCredito { get; set; }

        [JsonProperty("codeReasonState")]
        public int CodigoMotivoEstado { get; set; }

        [JsonProperty("observation")]
        public string? Observacion { get; set; }

        [JsonProperty("user")]
        public string? Usuario { get; set; }

        [JsonProperty("userCode")]
        public string? CodigoUsuario { get; set; }

        [JsonProperty("amountPrevious")]
        public decimal MontoAnterior { get; set; }

        [JsonProperty("flagValidated")]
        public string? FlagValidado { get; set; }

        [JsonProperty("acceptantLineNumber")]
        public string? NumeroLineaAceptante { get; set; }

        [JsonProperty("acceptantCustomerId")]
        public string? CodigoUnicoAceptante { get; set; }

        [JsonProperty("dateProcess")]
        public DateTime FechaProceso { get; set; }

        [JsonProperty("periodGrace")]
        public string? PeriodoGracia { get; set; }

        [JsonProperty("amortizationAmount")]
        public decimal MontoAmortizacion { get; set; }

        [JsonProperty("renewable")]
        public int Renovable { get; set; }

        [JsonProperty("renewalTerm")]
        public int PlazoDiasRenovable { get; set; }

        [JsonProperty("disbursements")]
        public int Desembolsos { get; set; }

        [JsonProperty("codeTypeScale")]
        public int CodigoTipoEscala { get; set; }

        [JsonProperty("descriptionTypeScale")]
        public string? DescripcionTipoEscala { get; set; }

        [JsonProperty("guaranteed100")]
        public int Garantizada100 { get; set; }

        [JsonProperty("flgPagare")]
        public int FlgPagare { get; set; }

        [JsonProperty("amountUsedSupplier")]
        public decimal MontoUtilizadoProveedor { get; set; }

        [JsonProperty("recordtype")]
        public string? TipoRegistro { get; set; }

        [JsonProperty("covenants")]
        public string? Covenants { get; set; }

        [JsonProperty("numberMaximumInstalments")]
        public int NumeroCuotasMaximo { get; set; }

        [JsonProperty("financing")]
        public string? Financiamiento { get; set; }

        [JsonProperty("SpecificOperation")]
        public decimal OpePuntuales { get; set; }

        [JsonProperty("amountUsedAcceptors")]
        public decimal MontoUtilizadoAceptantes { get; set; }

        [JsonProperty("mdcPlzOperations")]
        public string? MdcPlzOperaciones { get; set; }

        [JsonProperty("codeStoreLine")]
        public int CodigoTiendaLinea { get; set; }

        [JsonProperty("rateCommission")]
        public List<RateCommission>? RateCommissions { get; set; }
    }

    public class RateCommission
    {
        [JsonProperty("typeRateCommission")]
        public int TipoTasaComision { get; set; }

        [JsonProperty("descriptionRateCommission")]
        public string? DescripcionTasaComision { get; set; }

        [JsonProperty("codeSubTypeRate")]
        public int CodigoSubTipoTasa { get; set; }

        [JsonProperty("descriptionSubTypeRate")]
        public string? DescripcionSubTipoTasa { get; set; }

        [JsonProperty("currencyCode")]
        public int CodigoMoneda { get; set; }

        [JsonProperty("currencyDescription")]
        public string? DescripcionMoneda { get; set; }

        [JsonProperty("valueRateCommission")]
        public decimal ValorTasaComision { get; set; }

        [JsonProperty("typeValue")]
        public string? TipoValor { get; set; }
    }

}

