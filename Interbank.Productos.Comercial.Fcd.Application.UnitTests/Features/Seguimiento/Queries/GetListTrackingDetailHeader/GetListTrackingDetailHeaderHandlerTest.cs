using AutoMapper;
using FluentAssertions;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions.IErrorHandling;
using Interbank.Productos.Comercial.Fcd.Application.Features.Seguimiento.Queries.GetListTrackingDetailHeader;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Moq;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Seguimiento.Queries.GetListTrackingDetailHeader
{
    public class GetListTrackingDetailHeaderHandlerTest
    {
        private readonly Mock<ISeguimientoRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IValidation> _validationMock;
        private readonly GetListTrackingDetailHeaderHandler _handler;

        public GetListTrackingDetailHeaderHandlerTest()
        {
            _repositoryMock = new Mock<ISeguimientoRepository>();
            _mapperMock = new Mock<IMapper>();
            _validationMock = new Mock<IValidation>();

            _handler = new GetListTrackingDetailHeaderHandler(
                _repositoryMock.Object,
                _mapperMock.Object,
                _validationMock.Object);
        }

        #region Constructor Tests (Branch Coverage: Guard Clauses)

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
        {
            FluentActions.Invoking(() =>
                new GetListTrackingDetailHeaderHandler(
                    null!,
                    _mapperMock.Object,
                    _validationMock.Object))
            .Should()
            .Throw<ArgumentNullException>()
            .WithParameterName("seguimientoRepository");
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenMapperIsNull()
        {
            FluentActions.Invoking(() =>
                new GetListTrackingDetailHeaderHandler(
                    _repositoryMock.Object,
                    null!,
                    _validationMock.Object))
            .Should()
            .Throw<ArgumentNullException>()
            .WithParameterName("mapper");
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenValidationIsNull()
        {
            FluentActions.Invoking(() =>
                new GetListTrackingDetailHeaderHandler(
                    _repositoryMock.Object,
                    _mapperMock.Object,
                    null!))
            .Should()
            .Throw<ArgumentNullException>()
            .WithParameterName("validation");
        }

        #endregion

        #region Handle Method Tests (Branch & Line Coverage)

        [Fact]
        public async Task Handle_ShouldCallValidation_BeforeAnyAction()
        {
            // Arrange
            var query = new GetListTrackingDetailQuery("2026-01-01", "2026-01-31", "file.csv", "100");

            // Configuramos el mock para que no lance excepción y el flujo continúe hasta el repo
            _repositoryMock.Setup(r => r.ObtenerSeguimientoDetalleCabecera(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>()))
                .ReturnsAsync(new List<DapperSeguimientoDetalleCabecera> { new() });

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _validationMock.Verify(v => v.ValidationExceptionIfThereAreErrors(), Times.Once);
        }

        [Theory]
        [InlineData(true)]  // Escenario: proveedores es null (Cubre rama proveedores == null)
        [InlineData(false)] // Escenario: proveedores.Count == 0 (Cubre rama proveedores.Count == 0)
        public async Task Handle_ShouldThrowNotFoundException_WhenDataIsNullOrEmpty(bool isNull)
        {
            // Arrange
            var query = new GetListTrackingDetailQuery("2026-01-01", "2026-01-31", null, null);
            List<DapperSeguimientoDetalleCabecera>? response = isNull
                ? null
                : new List<DapperSeguimientoDetalleCabecera>();

            _repositoryMock.Setup(r => r.ObtenerSeguimientoDetalleCabecera(
                query.fechaDesde, query.fechaHasta, query.nombreArchivo, query.numeroPlanilla))
                .ReturnsAsync(response!);

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("NO HAY INFORMACIÓN RELACIONADA A LA CONSULTA");
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedList_WhenRepositoryReturnsData()
        {
            // Arrange
            var query = new GetListTrackingDetailQuery("2026-01-01", "2026-01-31", "test.csv", "123");
            var dapperData = new List<DapperSeguimientoDetalleCabecera>
            {
                new() { IdSeguimiento = 1, NombreArchivo = "test.csv" }
            };
            var expectedVm = new List<DetalleCabeceraVM>
            {
                new() { IdSeguimiento = 1, NombreArchivo = "test.csv" }
            };

            _repositoryMock.Setup(r => r.ObtenerSeguimientoDetalleCabecera(
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>()))
                .ReturnsAsync(dapperData);

            _mapperMock.Setup(m => m.Map<List<DetalleCabeceraVM>>(dapperData))
                       .Returns(expectedVm);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.Should().BeEquivalentTo(expectedVm);
            _repositoryMock.Verify(r => r.ObtenerSeguimientoDetalleCabecera(query.fechaDesde, query.fechaHasta, query.nombreArchivo, query.numeroPlanilla), Times.Once);
        }

        #endregion
    }
}
