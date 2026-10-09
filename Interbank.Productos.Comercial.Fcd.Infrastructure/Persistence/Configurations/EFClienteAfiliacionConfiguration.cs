using Interbank.Productos.Comercial.Fcd.Domain.Entities.EntityFramework;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence.Configurations
{
    public class EFClienteAfiliacionConfiguration : IEntityTypeConfiguration<EFClienteAfiliacion>
    {
        public void Configure(EntityTypeBuilder<EFClienteAfiliacion> modelBuilder)
        {
            modelBuilder.HasKey(ca => ca.CodigoAfiliacion);
            modelBuilder.ToTable("CLIENTEAFILIACION");
            modelBuilder.Property(ca => ca.DocDuplicado).HasColumnName("DOCDUPLICADO").HasColumnType(OracleColumnTypes.Integer);
            modelBuilder.Property(ca => ca.TipoMaxLote).HasColumnName("TIPOMAXLOTE").HasColumnType(OracleColumnTypes.Integer);
            modelBuilder.Property(ca => ca.TipoMaxProv).HasColumnName("TIPOMAXPROV").HasColumnType(OracleColumnTypes.Integer);
            modelBuilder.Property(ca => ca.MontoMaxLote).HasColumnName("MONTOMAXLOTE").HasColumnType(OracleColumnTypes.Number_20_2);
            modelBuilder.Property(ca => ca.MontoMaxProv).HasColumnName("MONTOMAXPROV").HasColumnType(OracleColumnTypes.Number_20_2);
            modelBuilder.Property(ca => ca.NombreContacto1).HasColumnName("NOMCONTACTO1").HasColumnType(OracleColumnTypes.Varchar150);
            modelBuilder.Property(ca => ca.EmailContacto1).HasColumnName("EMAILCONTACTO1").HasColumnType(OracleColumnTypes.Varchar100);
            modelBuilder.Property(ca => ca.CargoContacto1).HasColumnName("CARGOCONTACTO1").HasColumnType(OracleColumnTypes.Varchar100);
            modelBuilder.Property(ca => ca.Telefono1).HasColumnName("TELEFONO1").HasColumnType(OracleColumnTypes.Varchar15);
            modelBuilder.Property(ca => ca.Telefono2).HasColumnName("TELEFONO2").HasColumnType(OracleColumnTypes.Varchar15);
            modelBuilder.Property(ca => ca.NombreContacto2).HasColumnName("NOMCONTACTO2").HasColumnType(OracleColumnTypes.Varchar150);
            modelBuilder.Property(ca => ca.EmailContacto2).HasColumnName("EMAILCONTACTO2").HasColumnType(OracleColumnTypes.Varchar100);
            modelBuilder.Property(ca => ca.NombreContacto3).HasColumnName("NOMCONTACTO3").HasColumnType(OracleColumnTypes.Varchar150);
            modelBuilder.Property(ca => ca.EmailContacto3).HasColumnName("EMAILCONTACTO3").HasColumnType(OracleColumnTypes.Varchar100);
            modelBuilder.Property(ca => ca.TipoComision).HasColumnName("TIPOCOMISION").HasColumnType(OracleColumnTypes.Integer);
            modelBuilder.Property(ca => ca.MontoComision).HasColumnName("MONTOCOMISION").HasColumnType(OracleColumnTypes.Number_20_2);
            modelBuilder.Property(ca => ca.AmpliacionPago).HasColumnName("AMPLIACIONPAGO").HasColumnType(OracleColumnTypes.Integer);
            modelBuilder.Property(ca => ca.CodigoUnicoAceptante).HasColumnName("CODIGOUNICOACEPTANTE").HasColumnType(OracleColumnTypes.Varchar10);
            modelBuilder.Property(ca => ca.RazonSocialAceptante).HasColumnName("RAZONSOCIALACEPTANTE").HasColumnType(OracleColumnTypes.Varchar100);
            modelBuilder.Property(ca => ca.NumeroLineaAceptante).HasColumnName("NROLINEAACEPTANTE").HasColumnType(OracleColumnTypes.Varchar15);
            modelBuilder.Property(ca => ca.MontoMinSolesAceptante).HasColumnName("MONTOMINSOLESACEPTANTE").HasColumnType(OracleColumnTypes.Number_20_2);
            modelBuilder.Property(ca => ca.MontoMinDolarAceptante).HasColumnName("MONTOMINDOLARACEPTANTE").HasColumnType(OracleColumnTypes.Number_20_2);
            modelBuilder.Property(ca => ca.CodigoCliente).HasColumnName("CODIGOCLIENTE").HasColumnType(OracleColumnTypes.Integer);
            modelBuilder.Property(ca => ca.CodigoProveedor).HasColumnName("CODIGOPROVEEDOR").HasColumnType(OracleColumnTypes.Integer);
            modelBuilder.Property(ca => ca.CodigoAfiliacion).HasColumnName("CODIGOAFILIACION").HasColumnType(OracleColumnTypes.Integer);
            modelBuilder.Property(ca => ca.CodigoProducto).HasColumnName("CODIGOPRODUCTO").HasColumnType(OracleColumnTypes.Integer);
            modelBuilder.Property(ca => ca.CodigoEstadoAfiliacion).HasColumnName("CODIGOESTADOAFILIACION").HasColumnType(OracleColumnTypes.Integer);
            modelBuilder.Property(ca => ca.TipoAfiliacion).HasColumnName("TIPOAFILIACION").HasColumnType(OracleColumnTypes.Integer);
            modelBuilder.Property(ca => ca.FechaIngreso).HasColumnName("FECHAINGRESO").HasColumnType(OracleColumnTypes.Date);
            modelBuilder.Property(ca => ca.CodigoUsuarioRegistro).HasColumnName("CODIGOUSUARIOREGISTRO").HasColumnType(OracleColumnTypes.Varchar30);
            modelBuilder.Property(ca => ca.FechaRegistro).HasColumnName("FECHAREGISTRO").HasColumnType(OracleColumnTypes.Date);
            modelBuilder.Property(ca => ca.EstadoProveedor).HasColumnName("ESTADOPROVEEDOR").HasColumnType(OracleColumnTypes.Integer);
            modelBuilder.Property(ca => ca.CodigoUnico).HasColumnName("CODIGOUNICO").HasColumnType(OracleColumnTypes.Varchar10);
            modelBuilder.Property(ca => ca.RazonSocial).HasColumnName("RAZONSOCIAL").HasColumnType(OracleColumnTypes.Varchar100);
            modelBuilder.Property(ca => ca.NumeroLinea).HasColumnName("NROLINEA").HasColumnType(OracleColumnTypes.Varchar15);
            modelBuilder.Property(ca => ca.CodigoTipoDocumento).HasColumnName("CODIGOTIPODOCUMENTO").HasColumnType(OracleColumnTypes.Integer);
            modelBuilder.Property(ca => ca.NumeroDocumento).HasColumnName("NUMERODOCUMENTO").HasColumnType(OracleColumnTypes.Varchar20);
            modelBuilder.Property(ca => ca.DocumentoAuxiliarCliente).HasColumnName("DOCUMENTOAUXILIARCLIENTE").HasColumnType(OracleColumnTypes.Varchar30);
            modelBuilder.Property(ca => ca.TipoMonedaSoles).HasColumnName("TIPOMONEDASOLES").HasColumnType(OracleColumnTypes.Integer);
            modelBuilder.Property(ca => ca.TipoMonedaDolar).HasColumnName("TIPOMONEDADOLAR").HasColumnType(OracleColumnTypes.Integer);
            modelBuilder.Property(ca => ca.NumeroCuentaSoles).HasColumnName("NROCUENTASOLES").HasColumnType(OracleColumnTypes.Varchar20);
            modelBuilder.Property(ca => ca.NumeroCuentaDolar).HasColumnName("NROCUENTADOLAR").HasColumnType(OracleColumnTypes.Varchar20);
            modelBuilder.Property(ca => ca.TasaSoles).HasColumnName("TASASOLES").HasColumnType(OracleColumnTypes.Number_20_2);
            modelBuilder.Property(ca => ca.TasaDolar).HasColumnName("TASADOLAR").HasColumnType(OracleColumnTypes.Number_20_2);
            modelBuilder.Property(ca => ca.ValidaCuentaSoles).HasColumnName("VALIDACUENTASOLES").HasColumnType(OracleColumnTypes.Integer);
            modelBuilder.Property(ca => ca.ValidaCuentaDolares).HasColumnName("VALIDACUENTADOLARES").HasColumnType(OracleColumnTypes.Integer);
            modelBuilder.Property(ca => ca.DesembolsoAutoSoles).HasColumnName("DESEMBOLSOAUTOSOLES").HasColumnType(OracleColumnTypes.Integer);
            modelBuilder.Property(ca => ca.DesembolsoAutoDolar).HasColumnName("DESEMBOLSOAUTODOLAR").HasColumnType(OracleColumnTypes.Integer);
            modelBuilder.Property(ca => ca.Portes).HasColumnName("PORTES").HasColumnType(OracleColumnTypes.Integer);
            modelBuilder.Property(ca => ca.NumeroLineaCliente).HasColumnName("NROLINEACLIENTE").HasColumnType(OracleColumnTypes.Varchar15);
            modelBuilder.Property(ca => ca.NumeroLineaProveedor).HasColumnName("NROLINEAPROVEEDOR").HasColumnType(OracleColumnTypes.Varchar15);
            modelBuilder.Property(ca => ca.FechaUltimaActualizacion).HasColumnName("FECHAULTIMAACTUALIZACION").HasColumnType(OracleColumnTypes.Date);
            modelBuilder.Property(ca => ca.FechaAfiliacion).HasColumnName("FECHAAFILIACION").HasColumnType(OracleColumnTypes.Date);
            modelBuilder.Property(ca => ca.FechaDesafiliacion).HasColumnName("FECHADESAFILIACION").HasColumnType(OracleColumnTypes.Date);
            modelBuilder.Property(ca => ca.TasaClienteSoles).HasColumnName("TASACLIENTESOLES").HasColumnType(OracleColumnTypes.Number_20_2);
            modelBuilder.Property(ca => ca.TasaClienteDolar).HasColumnName("TASACLIENTEDOLAR").HasColumnType(OracleColumnTypes.Number_20_2);
        }
    }
}
