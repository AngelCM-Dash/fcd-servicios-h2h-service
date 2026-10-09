namespace Interbank.Productos.Comercial.Fcd.Application.Constant
{
    public static class Constante
    {
        public const int LongitudMaxCodigoUnico = 10;

        public enum Producto
        {
            Todos = 0,
            CobranzaVirtual = 22,
            CobranzaLibreLetrasxAceptar = 23,
            CobranzaLibreLetras = 24,
            CobranzaLibreFacturas = 25,
            CobranzaGarantiaLetras = 26,
            CobranzaGarantiaFacturas = 27,
            DescuentoLetras = 28,
            DescuentoLetrasLP = 29,
            DescuentoFacturas = 30,
            DescuentoElectFact = 31,
            FactoringFisicoFact = 32,
            FactoringElectrLetras = 33,
            FactoringElectrFact = 34,
            FactoringInterNacExport = 35,
            ForfaitingExportacion = 36,
            FactoringFisicoLetras = 60,
            FactoringInterNacImport = 63,
            Confirming = 64,
            DescuentoElectVirtual = 65,
            DescuentoFactNegociable = 66,
            CobranzaFactNegociable = 71,
            FactoringElectExpo = 75,
            Alternativo = 99
        }

    }
}
