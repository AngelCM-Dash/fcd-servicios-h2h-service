using FluentAssertions;
using Interbank.Productos.Comercial.Fcd.Application.Constant;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Monitor;
using Moq;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.UnitTests.Services.Monitor
{
    public class ArchivoMonitorServiceTest
  {
        private readonly Mock<ISftpService> _sftpServiceMock;
        private readonly Mock<IPlanillasRepository> _planillasRepositoryMock;
    private readonly Mock<IUtilitariosRepository> _utilitariosRepositoryMock;
        private readonly Mock<ITrazaService> _trazaServiceMock;
private readonly ArchivoMonitorService _service;

        public ArchivoMonitorServiceTest()
        {
     _sftpServiceMock = new Mock<ISftpService>();
            _planillasRepositoryMock = new Mock<IPlanillasRepository>();
            _utilitariosRepositoryMock = new Mock<IUtilitariosRepository>();
  _trazaServiceMock = new Mock<ITrazaService>();

        _service = new ArchivoMonitorService(
    _sftpServiceMock.Object,
                _planillasRepositoryMock.Object,
        _utilitariosRepositoryMock.Object,
    _trazaServiceMock.Object);
        }

        /// <summary>
        /// Builds the minimal list of DapperParametro entries required by ObtenerConfiguracionSftpMonitor.
      /// </summary>
        private static List<DapperParametro> BuildSftpMonitorParams(
          string host = "sftp.test.com",
    string port = "22",
         string user = "usr",
        string pass = "pwd",
       string ppk = "/keys/key.ppk",
     string flag = "True",
       string remotePath = "/remote/path/")
        {
            return new List<DapperParametro>
            {
            new() { NUMEROORDEN = (int)ParametroConstants.NumOrdenDominioSftpMonitor.SftpHost,          DESCRIPCIONCORTA = host },
        new() { NUMEROORDEN = (int)ParametroConstants.NumOrdenDominioSftpMonitor.SftpPort,       DESCRIPCIONCORTA = port },
              new() { NUMEROORDEN = (int)ParametroConstants.NumOrdenDominioSftpMonitor.SftpUsername,      DESCRIPCIONCORTA = user },
   new() { NUMEROORDEN = (int)ParametroConstants.NumOrdenDominioSftpMonitor.SftpPassword,      DESCRIPCIONCORTA = pass },
     new() { NUMEROORDEN = (int)ParametroConstants.NumOrdenDominioSftpMonitor.SftpRemotePathKey, DESCRIPCIONCORTA = ppk },
            new() { NUMEROORDEN = (int)ParametroConstants.NumOrdenDominioSftpMonitor.FlagRed,  DESCRIPCIONCORTA = flag },
    new() { NUMEROORDEN = (int)ParametroConstants.NumOrdenDominioSftpMonitor.SftpRemotePath,    DESCRIPCIONCORTA = remotePath },
   };
  }

        private void SetupSftpOperations()
     {
            _sftpServiceMock.Setup(s => s.Conectar(
   It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(),
           It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
 .Returns(Task.CompletedTask);

            _sftpServiceMock.Setup(s => s.SubirArchivoSftp(It.IsAny<Stream>(), It.IsAny<string>()))
          .Returns(Task.CompletedTask);

       _sftpServiceMock.Setup(s => s.Desconectar())
      .Returns(Task.CompletedTask);
        }

 private void SetupTrazaService()
        {
            _trazaServiceMock.Setup(t => t.RegistrarDetalleTraza(
          It.IsAny<decimal>(), It.IsAny<string>(),
    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
           .ReturnsAsync(1m);
 }

        #region Constructor

        [Fact]
   public void Constructor_CuandoDependenciasValidas_CreaInstanciaCorrectamente()
        {
   // Arrange & Act
            var service = new ArchivoMonitorService(
         _sftpServiceMock.Object,
    _planillasRepositoryMock.Object,
     _utilitariosRepositoryMock.Object,
   _trazaServiceMock.Object);

      // Assert
   service.Should().NotBeNull();
        }

        #endregion

        #region GenerarArchivoPlano

 [Fact]
  public async Task GenerarArchivoPlano_Flujo1SinDetalle_RetornaRutaConNombreArchivo()
        {
   // Arrange
            const string planilla = "PL0000001";
      const string observacion = ProcesarTramasConstants.DesembolsoOK;
     const int flujo = 1;
            const int idCabecera = 10;

            SetupTrazaService();

            _planillasRepositoryMock
          .Setup(r => r.ObtenerInformacionPlanilla(planilla))
   .ReturnsAsync(new DapperPlanillaCompleta { CanalAtencion = "H2H" });

            _utilitariosRepositoryMock
         .Setup(r => r.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioSftpMonitorFCD))
      .ReturnsAsync(BuildSftpMonitorParams());

        SetupSftpOperations();

  // Act
            var result = await _service.GenerarArchivoPlano(planilla, observacion, flujo, idCabecera);

            // Assert
  result.Should().StartWith("ruta/remota/");
            result.Should().Contain("MONITOR_H2H_");
         result.Should().Contain(planilla);
            result.Should().Contain("H2H");
   }

        [Fact]
  public async Task GenerarArchivoPlano_Flujo2ConDetalle_LlamaObtenerDetallePlanillaMonitor()
        {
     // Arrange
            const string planilla = "PL0000002";
  const int flujo = 2;
        const int idCabecera = 11;

       SetupTrazaService();

   _planillasRepositoryMock
    .Setup(r => r.ObtenerInformacionPlanilla(planilla))
        .ReturnsAsync(new DapperPlanillaCompleta { CanalAtencion = "FCD" });

      _planillasRepositoryMock
    .Setup(r => r.ObtenerDetallePlanillaMonitor(planilla))
   .ReturnsAsync(new List<DapperDetallePlanillaMonitor>
              {
      new()
           {
        NumeroInterno        = "INT001",
             NumeroDocumentoAceptante = "12345678901234",
      TipoOperacion        = "CR",
           NumeroDocumentoFisico = "DOC0001",
               Estado             = "VIGENTE",
         Observacion          = "-"
}
            });

            _utilitariosRepositoryMock
    .Setup(r => r.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioSftpMonitorFCD))
             .ReturnsAsync(BuildSftpMonitorParams());

       SetupSftpOperations();

          // Act
        var result = await _service.GenerarArchivoPlano(planilla, null, flujo, idCabecera);

// Assert
            result.Should().StartWith("ruta/remota/");
            _planillasRepositoryMock.Verify(r => r.ObtenerDetallePlanillaMonitor(planilla), Times.Once);
        }

  [Fact]
        public async Task GenerarArchivoPlano_Flujo2ConDetalleEstadoNoVigente_LlamaObtenerCodigoDetalle()
 {
            // Arrange
      const string planilla = "PL0000003";
        const int flujo = 2;
 const int idCabecera = 12;

       SetupTrazaService();

 _planillasRepositoryMock
  .Setup(r => r.ObtenerInformacionPlanilla(planilla))
       .ReturnsAsync(new DapperPlanillaCompleta { CanalAtencion = "FCD" });

   _planillasRepositoryMock
        .Setup(r => r.ObtenerDetallePlanillaMonitor(planilla))
  .ReturnsAsync(new List<DapperDetallePlanillaMonitor>
       {
                    new()
               {
             NumeroInterno   = "INT002",
               NumeroDocumentoAceptante = "99999999999999",
          TipoOperacion    = "DB",
     NumeroDocumentoFisico    = "DOC0002",
   Estado     = "RECHAZADO",
               Observacion     = "SALDO INSUFICIENTE"
              }
      });

            _utilitariosRepositoryMock
          .Setup(r => r.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioSftpMonitorFCD))
                .ReturnsAsync(BuildSftpMonitorParams());

       SetupSftpOperations();

     // Act
          var result = await _service.GenerarArchivoPlano(planilla, null, flujo, idCabecera);

     // Assert
   result.Should().StartWith("ruta/remota/");
   _planillasRepositoryMock.Verify(r => r.ObtenerDetallePlanillaMonitor(planilla), Times.Once);
      }

        [Fact]
      public async Task GenerarArchivoPlano_ObservacionNula_GeneraCabeceraConCodigo00099()
        {
    // Arrange
            const string planilla = "PL0000004";
            const int flujo = 1;
         const int idCabecera = 13;

          SetupTrazaService();

            _planillasRepositoryMock
      .Setup(r => r.ObtenerInformacionPlanilla(planilla))
     .ReturnsAsync(new DapperPlanillaCompleta { CanalAtencion = "H2H" });

          _utilitariosRepositoryMock
       .Setup(r => r.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioSftpMonitorFCD))
         .ReturnsAsync(BuildSftpMonitorParams());

          SetupSftpOperations();

   // Act — observacionCabecera = null -> ObtenerCodigoCabecera("") -> CodigoDefault "00000"
            var result = await _service.GenerarArchivoPlano(planilla, null, flujo, idCabecera);

            // Assert
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GenerarArchivoPlano_CualquierFlujo_RegistraInicioYFinDeTraza()
        {
    // Arrange
            const string planilla = "PL0000005";
     const int idCabecera = 14;

            SetupTrazaService();

     _planillasRepositoryMock
         .Setup(r => r.ObtenerInformacionPlanilla(planilla))
  .ReturnsAsync(new DapperPlanillaCompleta { CanalAtencion = "H2H" });

   _utilitariosRepositoryMock
         .Setup(r => r.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioSftpMonitorFCD))
     .ReturnsAsync(BuildSftpMonitorParams());

      SetupSftpOperations();

        // Act
await _service.GenerarArchivoPlano(planilla, ProcesarTramasConstants.DesembolsoOK, 1, idCabecera);

 // Assert — debe registrar al menos 2 veces (inicio + fin)
   _trazaServiceMock.Verify(t => t.RegistrarDetalleTraza(
        idCabecera,
              nameof(ArchivoMonitorService),
                It.IsAny<string>(),
          It.IsAny<string>(),
    13), Times.AtLeast(2));
    }

        [Fact]
        public async Task GenerarArchivoPlano_CualquierFlujo_ConectaYDesconectaSftp()
        {
            // Arrange
          const string planilla = "PL0000006";
            const int idCabecera = 15;

            SetupTrazaService();

 _planillasRepositoryMock
         .Setup(r => r.ObtenerInformacionPlanilla(planilla))
 .ReturnsAsync(new DapperPlanillaCompleta { CanalAtencion = "H2H" });

    // FlagRed = "True" ? Convert.ToBoolean("True") = true ? Flag stored as "1" ? Conectar receives true
          _utilitariosRepositoryMock
      .Setup(r => r.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioSftpMonitorFCD))
  .ReturnsAsync(BuildSftpMonitorParams(
             host: "sftp.test.com",
 port: "22",
    user: "usr",
         pass: "pwd",
    ppk: "/keys/key.ppk",
                    flag: "True",
              remotePath: "/remote/path/"));

   SetupSftpOperations();

            // Act
     await _service.GenerarArchivoPlano(planilla, ProcesarTramasConstants.DesembolsoOK, 1, idCabecera);

         // Assert
            _sftpServiceMock.Verify(s => s.Conectar(
                "sftp.test.com", 22, "usr", "pwd", "/keys/key.ppk", true), Times.Once);
     _sftpServiceMock.Verify(s => s.SubirArchivoSftp(
       It.IsAny<Stream>(),
           It.Is<string>(r => r.StartsWith("/remote/path/MONITOR_H2H_"))), Times.Once);
      _sftpServiceMock.Verify(s => s.Desconectar(), Times.Once);
 }

   [Fact]
        public async Task GenerarArchivoPlano_Flujo1_NoLlamaObtenerDetallePlanillaMonitor()
        {
 // Arrange
          const string planilla = "PL0000007";
            const int flujo = 1;
    const int idCabecera = 16;

   SetupTrazaService();

    _planillasRepositoryMock
           .Setup(r => r.ObtenerInformacionPlanilla(planilla))
         .ReturnsAsync(new DapperPlanillaCompleta { CanalAtencion = "H2H" });

            _utilitariosRepositoryMock
        .Setup(r => r.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioSftpMonitorFCD))
    .ReturnsAsync(BuildSftpMonitorParams());

            SetupSftpOperations();

            // Act
 await _service.GenerarArchivoPlano(planilla, ProcesarTramasConstants.DesembolsoOK, flujo, idCabecera);

       // Assert
 _planillasRepositoryMock.Verify(r => r.ObtenerDetallePlanillaMonitor(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GenerarArchivoPlano_Flujo2ListaDetalleVacia_SubeArchivoSoloConCabecera()
        {
    // Arrange
         const string planilla = "PL0000008";
        const int flujo = 2;
            const int idCabecera = 17;

            SetupTrazaService();

            _planillasRepositoryMock
             .Setup(r => r.ObtenerInformacionPlanilla(planilla))
      .ReturnsAsync(new DapperPlanillaCompleta { CanalAtencion = "H2H" });

            _planillasRepositoryMock
  .Setup(r => r.ObtenerDetallePlanillaMonitor(planilla))
           .ReturnsAsync(new List<DapperDetallePlanillaMonitor>());

         _utilitariosRepositoryMock
     .Setup(r => r.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioSftpMonitorFCD))
     .ReturnsAsync(BuildSftpMonitorParams());

            SetupSftpOperations();

        // Act
     var result = await _service.GenerarArchivoPlano(planilla, ProcesarTramasConstants.DesembolsoOK, flujo, idCabecera);

 // Assert
  result.Should().StartWith("ruta/remota/");
   _sftpServiceMock.Verify(s => s.SubirArchivoSftp(It.IsAny<Stream>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GenerarArchivoPlano_FlagRedTrue_LlamaSftpConFlagClaveTrueAndViceversa()
{
            // Arrange
     const string planilla = "PL0000009";
            const int idCabecera = 18;

            SetupTrazaService();

            _planillasRepositoryMock
        .Setup(r => r.ObtenerInformacionPlanilla(planilla))
  .ReturnsAsync(new DapperPlanillaCompleta { CanalAtencion = "H2H" });

            // FlagRed = "False" ? Convert.ToBoolean("False") = false ? Flag stored as "0" ? "0" == "1" = false
      _utilitariosRepositoryMock
     .Setup(r => r.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioSftpMonitorFCD))
    .ReturnsAsync(BuildSftpMonitorParams(flag: "False"));

     SetupSftpOperations();

            // Act
        await _service.GenerarArchivoPlano(planilla, ProcesarTramasConstants.DesembolsoOK, 1, idCabecera);

       // Assert — flagClave must be false when FlagRed = "False"
    _sftpServiceMock.Verify(s => s.Conectar(
      It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), false), Times.Once);
        }

        #endregion

  #region ObtenerCodigoDetalle

        [Theory]
        [InlineData("", ProcesarTramasConstants.CodigoDefault)]
        [InlineData("-", ProcesarTramasConstants.CodigoDefault)]
        [InlineData("--", ProcesarTramasConstants.CodigoDefault)]
        [InlineData("---", ProcesarTramasConstants.CodigoDefault)]
    [InlineData("COMMAREA TLDO087", "00001")]
        [InlineData("ABEND AAL8 EN PROGRAMA TLDOCLI", "00002")]
        [InlineData("ARCHIVO NO ABIERTO", "00003")]
      [InlineData("CJE - 2042", "00004")]
        [InlineData("CJE - 2042BBVA", "00005")]
        [InlineData("CLAVE NO EXISTE,VERIFIQUE", "00006")]
        [InlineData("CTA CTE PURGADA.VERIFICAR", "00007")]
        [InlineData("ERROR EN RECURSO TLDFDDUS 9770FCDE0002 S", "00008")]
        [InlineData("ERROR EN RECURSO TLDFDDUS 9770FCDE0003 S", "00009")]
        [InlineData("ERROR EN RECURSO TLDFDDUS 9770FCDE0004 S", "00010")]
      [InlineData("ERROR EN RECURSO TLDFDDUS 9770FCDE0008 S", "00011")]
      [InlineData("ERROR EN TIPO OPERACION CR/DB", "00012")]
        [InlineData("ERROR TRAMA SALIDA SYSTEMATICS", "00013")]
    [InlineData("NO CUMPLE DIGITO VERIFI.", "00014")]
        [InlineData("NO DEFINIDO", "00015")]
      [InlineData("OML0 TS9951 F: ARCHIVO ARCHIVO CERRADO I", "00016")]
        [InlineData("RECURSO NO DISPONIBLE APLICAC. DEP", "00017")]
[InlineData("SALDO INSUFICIENTE", "00018")]
        public void ObtenerCodigoDetalle_ObservacionConocida_RetornaCodigoEsperado(string observacion, string codigoEsperado)
  {
            // Arrange & Act
      var resultado = ArchivoMonitorService.ObtenerCodigoDetalle(observacion);

    // Assert
        resultado.Should().Be(codigoEsperado);
        }

        [Theory]
        [InlineData("OBSERVACION DESCONOCIDA")]
        [InlineData("CUALQUIER OTRO ERROR")]
        [InlineData("XYZ")]
  public void ObtenerCodigoDetalle_ObservacionDesconocida_Retorna00099(string observacion)
        {
            // Arrange & Act
     var resultado = ArchivoMonitorService.ObtenerCodigoDetalle(observacion);

      // Assert
            resultado.Should().Be("00099");
     }

        #endregion

        #region ObtenerCodigoCabecera

        [Theory]
        [InlineData(ProcesarTramasConstants.DesembolsoOK, ProcesarTramasConstants.CodigoDefault)]
        [InlineData(ProcesarTramasConstants.ErrorRegistrarDetallePlanilla, "00001")]
        [InlineData(ProcesarTramasConstants.ErrorRegistroTramas, "00002")]
        [InlineData(ProcesarTramasConstants.ErrorActualizarEstadoPlanillaDb2, "00003")]
        [InlineData(ProcesarTramasConstants.ErrorRegistrarMovimientoDocumentos, "00004")]
     public void ObtenerCodigoCabecera_ObservacionConocida_RetornaCodigoEsperado(string observacion, string codigoEsperado)
        {
     // Arrange & Act
            var resultado = ArchivoMonitorService.ObtenerCodigoCabecera(observacion);

         // Assert
            resultado.Should().Be(codigoEsperado);
      }

        [Theory]
        [InlineData("OBSERVACION NO MAPEADA")]
        [InlineData("ERROR GENERICO")]
        [InlineData("CUALQUIER TEXTO")]
        public void ObtenerCodigoCabecera_ObservacionDesconocida_Retorna00099(string observacion)
   {
            // Arrange & Act
        var resultado = ArchivoMonitorService.ObtenerCodigoCabecera(observacion);

 // Assert
  resultado.Should().Be("00099");
        }

        [Fact]
        public void ObtenerCodigoCabecera_ObservacionVacia_Retorna00099()
        {
          // Arrange & Act
 var resultado = ArchivoMonitorService.ObtenerCodigoCabecera(string.Empty);

            // Assert
            resultado.Should().Be("00099");
  }

        #endregion

        #region GenerarArchivoPlano - Nombre de archivo generado

        [Fact]
  public async Task GenerarArchivoPlano_Flujo1_NombreArchivoContieneTimestampPlanillaYCanal()
   {
            // Arrange
            const string planilla = "PL9999999";
     const string canal = "WBC";
            const int idCabecera = 20;

          SetupTrazaService();

         _planillasRepositoryMock
         .Setup(r => r.ObtenerInformacionPlanilla(planilla))
                .ReturnsAsync(new DapperPlanillaCompleta { CanalAtencion = canal });

          _utilitariosRepositoryMock
    .Setup(r => r.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioSftpMonitorFCD))
                .ReturnsAsync(BuildSftpMonitorParams());

            SetupSftpOperations();

        // Act
     var result = await _service.GenerarArchivoPlano(planilla, ProcesarTramasConstants.DesembolsoOK, 1, idCabecera);

            // Assert — format: ruta/remota/MONITOR_H2H_{timestamp}_{planilla}_{canal}.txt
            result.Should().MatchRegex(@"ruta/remota/MONITOR_H2H_\d{17}_PL9999999_WBC\.txt");
        }

        #endregion

      #region GenerarArchivoPlano - Configuración SFTP Monitor

        [Fact]
     public async Task GenerarArchivoPlano_CualquierFlujo_ConsultaParametrosDominioSftpMonitorFCD()
        {
    // Arrange
            const string planilla = "PL0001111";
         const int idCabecera = 21;

     SetupTrazaService();

            _planillasRepositoryMock
          .Setup(r => r.ObtenerInformacionPlanilla(planilla))
           .ReturnsAsync(new DapperPlanillaCompleta { CanalAtencion = "H2H" });

            _utilitariosRepositoryMock
     .Setup(r => r.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioSftpMonitorFCD))
       .ReturnsAsync(BuildSftpMonitorParams());

        SetupSftpOperations();

         // Act
       await _service.GenerarArchivoPlano(planilla, ProcesarTramasConstants.DesembolsoOK, 1, idCabecera);

     // Assert
            _utilitariosRepositoryMock.Verify(r =>
          r.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioSftpMonitorFCD), Times.Once);
}

        [Fact]
        public async Task GenerarArchivoPlano_CualquierFlujo_UsaRutaRemotaDeParametros()
      {
            // Arrange
            const string planilla = "PL0002222";
   const string remotePath = "/custom/remote/path/";
            const int idCabecera = 22;

  SetupTrazaService();

            _planillasRepositoryMock
  .Setup(r => r.ObtenerInformacionPlanilla(planilla))
      .ReturnsAsync(new DapperPlanillaCompleta { CanalAtencion = "H2H" });

_utilitariosRepositoryMock
    .Setup(r => r.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioSftpMonitorFCD))
   .ReturnsAsync(BuildSftpMonitorParams(remotePath: remotePath));

          SetupSftpOperations();

       // Act
            await _service.GenerarArchivoPlano(planilla, ProcesarTramasConstants.DesembolsoOK, 1, idCabecera);

       // Assert
 _sftpServiceMock.Verify(s => s.SubirArchivoSftp(
     It.IsAny<Stream>(),
           It.Is<string>(r => r.StartsWith(remotePath))), Times.Once);
    }

        [Fact]
    public async Task GenerarArchivoPlano_CualquierFlujo_ParametrosSftpNulos_UsaStringEmptyEnConectar()
    {
        // Arrange
        // An empty list makes every Find(...) return null, covering the null branch
        // of every ?.DESCRIPCIONCORTA operator in ObtenerConfiguracionSftpMonitor:
        //   password?.DESCRIPCIONCORTA   ? null ? Password    = null
        //   host?.DESCRIPCIONCORTA       ? null ? Host        = null
        //   port?.DESCRIPCIONCORTA       ? null ? Convert.ToInt32(null)   = 0
        //   username?.DESCRIPCIONCORTA   ? null ? Usuario     = null
  //   privateKey?.DESCRIPCIONCORTA ? null ? ArchivoPpk  = null
        //   flag?.DESCRIPCIONCORTA       ? null ? Convert.ToBoolean(null) = false ? Flag = "0"
   //   remotePath?.DESCRIPCIONCORTA ? null ? RutaArchivo = null
        // Then the ?? string.Empty fallbacks in Conectar() produce string.Empty for all strings.
  const string planilla = "PL0005555";
  const int idCabecera = 25;

     SetupTrazaService();

        _planillasRepositoryMock
       .Setup(r => r.ObtenerInformacionPlanilla(planilla))
            .ReturnsAsync(new DapperPlanillaCompleta { CanalAtencion = "H2H" });

  // Empty list ? all Find() calls return null ? all ?. null branches hit
        _utilitariosRepositoryMock
            .Setup(r => r.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioSftpMonitorFCD))
      .ReturnsAsync(new List<DapperParametro>());

     SetupSftpOperations();

        // Act
      await _service.GenerarArchivoPlano(planilla, ProcesarTramasConstants.DesembolsoOK, 1, idCabecera);

        // Assert — every nullable field falls back via ?? string.Empty in ObtenerConfiguracionSftpMonitor;
    //          port = Convert.ToInt32(null) = 0; flagClave = "0" == "1" = false
        // ConfigSftpRequest properties are already string.Empty, so Conectar receives them directly.
        _sftpServiceMock.Verify(s => s.Conectar(
   string.Empty,  // host      = string.Empty (from host ?? string.Empty in ObtenerConfiguracionSftpMonitor)
         0, // Convert.ToInt32(null) = 0
            string.Empty,  // usuario   = string.Empty
 string.Empty,  // password  = string.Empty
      string.Empty,  // archivoPpk = string.Empty
    false),        // Convert.ToBoolean(null) = false ? "0" == "1" = false
            Times.Once);
    }

    #endregion

  #region GenerarArchivoPlano - Detalle con múltiples registros

    [Fact]
        public async Task GenerarArchivoPlano_Flujo2MultipleDetalles_ProcesaTodosLosRegistros()
        {
   // Arrange
     const string planilla = "PL0003333";
  const int flujo = 2;
            const int idCabecera = 23;

            SetupTrazaService();

            _planillasRepositoryMock
       .Setup(r => r.ObtenerInformacionPlanilla(planilla))
              .ReturnsAsync(new DapperPlanillaCompleta { CanalAtencion = "H2H" });

            _planillasRepositoryMock
           .Setup(r => r.ObtenerDetallePlanillaMonitor(planilla))
              .ReturnsAsync(new List<DapperDetallePlanillaMonitor>
          {
        new() { NumeroInterno = "INT001", NumeroDocumentoAceptante = "111", TipoOperacion = "CR", NumeroDocumentoFisico = "DOC1", Estado = "VIGENTE",   Observacion = "-" },
     new() { NumeroInterno = "INT002", NumeroDocumentoAceptante = "222", TipoOperacion = "DB", NumeroDocumentoFisico = "DOC2", Estado = "RECHAZADO", Observacion = "SALDO INSUFICIENTE" },
new() { NumeroInterno = "INT003", NumeroDocumentoAceptante = "333", TipoOperacion = "CR", NumeroDocumentoFisico = "DOC3", Estado = "RECHAZADO", Observacion = "NO DEFINIDO" },
  });

         _utilitariosRepositoryMock
     .Setup(r => r.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioSftpMonitorFCD))
        .ReturnsAsync(BuildSftpMonitorParams());

            SetupSftpOperations();

            // Act
    var result = await _service.GenerarArchivoPlano(planilla, ProcesarTramasConstants.DesembolsoOK, flujo, idCabecera);

// Assert
 result.Should().StartWith("ruta/remota/");
            _sftpServiceMock.Verify(s => s.SubirArchivoSftp(It.IsAny<Stream>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GenerarArchivoPlano_Flujo2DetalleConCamposNulos_NoLanzaExcepcion()
     {
    // Arrange
            const string planilla = "PL0004444";
          const int flujo = 2;
         const int idCabecera = 24;

  SetupTrazaService();

        _planillasRepositoryMock
      .Setup(r => r.ObtenerInformacionPlanilla(planilla))
      .ReturnsAsync(new DapperPlanillaCompleta { CanalAtencion = "H2H" });

      _planillasRepositoryMock
     .Setup(r => r.ObtenerDetallePlanillaMonitor(planilla))
                .ReturnsAsync(new List<DapperDetallePlanillaMonitor>
    {
    // All optional fields are null
new() { NumeroInterno = null, NumeroDocumentoAceptante = null, TipoOperacion = null, NumeroDocumentoFisico = null, Estado = null, Observacion = null }
      });

          _utilitariosRepositoryMock
           .Setup(r => r.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioSftpMonitorFCD))
     .ReturnsAsync(BuildSftpMonitorParams());

            SetupSftpOperations();

    // Act
     var act = async () => await _service.GenerarArchivoPlano(planilla, null, flujo, idCabecera);

            // Assert
          await act.Should().NotThrowAsync();
     }

        #endregion
    }
}
