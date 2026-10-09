namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.DataTable.DB2
{
    public class ColumnMapping
    {
        public string NombreColumna { get; }
        public int IndiceComienzo { get; }
        public int Longitud { get; }
        public Func<string, object> Conversion { get; }

        public ColumnMapping(string nombreColumna, int indiceComienzo, int longitud, Func<string, object> conversion)
        {
            NombreColumna = nombreColumna;
            IndiceComienzo = indiceComienzo;
            Longitud = longitud;
            Conversion = conversion;
        }
    }
}
