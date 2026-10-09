using AutoMapper;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persintence;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions.IErrorHandling;
using Interbank.Productos.Comercial.Fcd.Application.Features.Clientes.Queries.GetSuppliersList;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.EntityFramework;
using Moq;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Clientes.Queries.GetSuppliersList
{
    public class GetSuppliersListHandlerTest
    {
        private readonly Mock<IClienteAfiliacionRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IValidation> _validationMock;
        private readonly GetSuppliersListHandler _handler;

        public GetSuppliersListHandlerTest()
        {
            _repositoryMock = new Mock<IClienteAfiliacionRepository>();
            _mapperMock = new Mock<IMapper>();
            _validationMock = new Mock<IValidation>();

            _handler = new GetSuppliersListHandler(
                _repositoryMock.Object,
                _mapperMock.Object,
                _validationMock.Object
            );
        }

        [Fact]
        public void Constructor_NullClienteAfiliacionRepository_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new GetSuppliersListHandler(null!, _mapperMock.Object, _validationMock.Object)
            );
            Assert.Equal("clienteAfiliacionRepository", ex.ParamName);
        }

        [Fact]
        public void Constructor_NullMapper_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new GetSuppliersListHandler(_repositoryMock.Object, null!, _validationMock.Object)
            );
            Assert.Equal("mapper", ex.ParamName);
        }

        [Fact]
        public void Constructor_NullValidation_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new GetSuppliersListHandler(_repositoryMock.Object, _mapperMock.Object, null!)
            );
            Assert.Equal("validation", ex.ParamName);
        }


        [Fact]
        public async Task Handle_CodigoUnicoNoNumerico_LlamaAddValidationFailure()
        {
            // Arrange
            var request = new GetSuppliersListQuery("ABC123", "31"); // Código único no numérico

            // Simula AddValidationFailure
            _validationMock.Setup(v => v.AddValidationFailure(
                It.IsAny<string>(),
                It.IsAny<string>()
            )).Callback<string, string>((key, msg) => { /* opcional: log */ });

            // Simula ValidationException con diccionario
            var errores = new Dictionary<string, List<string>>
            {
                { nameof(request.CodigoUnicoAceptante), new List<string> { $"EL CÓDIGO ÚNICO '{request.CodigoUnicoAceptante}' NO TIENE UN FORMATO VÁLIDO." } }
            };
            _validationMock.Setup(v => v.ValidationExceptionIfThereAreErrors())
                           .Throws(new ValidationException(errores));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
                await _handler.Handle(request, CancellationToken.None)
            );

            // Verifica que la excepción contiene la clave esperada
            Assert.True(ex.Errors.ContainsKey(nameof(request.CodigoUnicoAceptante)));
            Assert.Contains("NO TIENE UN FORMATO VÁLIDO", ex.Errors[nameof(request.CodigoUnicoAceptante)].First());

            // Verifica que AddValidationFailure fue llamado
            _validationMock.Verify(v => v.AddValidationFailure(
                nameof(request.CodigoUnicoAceptante),
                It.Is<string>(s => s.Contains("NO TIENE UN FORMATO VÁLIDO"))
            ), Times.Once);
        }


        [Fact]
        public async Task Handle_CodigoUnicoMuyLargo_LlamaAddValidationFailure()
        {
            // Arrange
            string largo = new string('1', 20); // mayor que LongitudMaxCodigoUnico
            var request = new GetSuppliersListQuery(largo, "31");

            var efProveedores = new List<EFClienteAfiliacion>
            {
                new EFClienteAfiliacion { DocDuplicado = 1 }
            };
            _repositoryMock.Setup(r => r.GetSuppliersByAcceptor(It.IsAny<string>(), It.IsAny<int?>()))
                           .ReturnsAsync(efProveedores);

            _validationMock.Setup(v => v.AddValidationFailure(
                nameof(request.CodigoUnicoAceptante),
                It.IsAny<string>()))
                .Verifiable();

            var errores = new Dictionary<string, List<string>>
            {
                { "CodigoUnicoAceptante", new List<string> { "EL CÓDIGO ÚNICO SUPERA LA LONGITUD ESTANDAR" } }
            };

            _validationMock.Setup(v => v.ValidationExceptionIfThereAreErrors())
                           .Throws(new ValidationException(errores));


            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(request, CancellationToken.None)
            );

            _validationMock.Verify(v => v.AddValidationFailure(
                nameof(request.CodigoUnicoAceptante),
                It.Is<string>(s => s.Contains("SUPERA LA LONGITUD ESTANDAR"))
            ), Times.Once);
        }


        [Fact]
        public async Task Handle_CodigoProductoNoNumerico_LlamaAddValidationFailure()
        {
            // Arrange
            var request = new GetSuppliersListQuery("123456", "ABC");

            // Simula que AddValidationFailure agrega errores al diccionario
            _validationMock.Setup(v => v.AddValidationFailure(
                It.IsAny<string>(),
                It.IsAny<string>()
            )).Callback<string, string>((key, message) =>
            {
                // opcional: aquí podrías almacenar errores si quieres verificarlos luego
            });

            // Simula que ValidationExceptionIfThereAreErrors lanza la excepción con diccionario
            var errores = new Dictionary<string, List<string>>
            {
                { nameof(request.CodigoProducto), new List<string> { "EL CÓDIGO DE PRODUCTO 'ABC' DEBE SER UN NÚMERO ENTERO." } }
            };
            _validationMock.Setup(v => v.ValidationExceptionIfThereAreErrors())
                           .Throws(new ValidationException(errores));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
                await _handler.Handle(request, CancellationToken.None)
            );

            // Opcional: verificar que el diccionario contenga la clave esperada
            Assert.True(ex.Errors.ContainsKey(nameof(request.CodigoProducto)));
            Assert.Contains("DEBE SER UN NÚMERO ENTERO", ex.Errors[nameof(request.CodigoProducto)].First());

            // Verifica que AddValidationFailure se haya llamado
            _validationMock.Verify(v => v.AddValidationFailure(
                nameof(request.CodigoProducto),
                It.Is<string>(s => s.Contains("DEBE SER UN NÚMERO ENTERO"))
            ), Times.Once);
        }

        [Fact]
        public async Task Handle_CodigoProductoEsNull_NoLlamaAddValidationFailure()
        {
            // Arrange
            var request = new GetSuppliersListQuery("1234567890", null);

            // Repositorio devuelve al menos un proveedor
            var proveedoresMock = new List<EFClienteAfiliacion>
            {
                new EFClienteAfiliacion { CodigoUnicoAceptante = "1234567890" }
            };

            _repositoryMock
                .Setup(r => r.GetSuppliersByAcceptor(request.CodigoUnicoAceptante, null))
                .ReturnsAsync(proveedoresMock);

            // Mapper para convertir EFClienteAfiliacion a ClienteAfiliacionVM
            _mapperMock
                .Setup(m => m.Map<List<ClienteAfiliacionVM>>(proveedoresMock))
                .Returns(new List<ClienteAfiliacionVM> { new ClienteAfiliacionVM() });

            _validationMock.Setup(v => v.ValidationExceptionIfThereAreErrors());

            // Act
            var ex = await Record.ExceptionAsync(() => _handler.Handle(request, CancellationToken.None));

            // Assert
            Assert.Null(ex); // ya no debería lanzar NotFoundException
            _validationMock.Verify(v => v.AddValidationFailure(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_CodigoProductoNoDefinido_LlamaAddValidationFailure()
        {
            // Arrange
            var request = new GetSuppliersListQuery("123456", "999"); // código producto no definido en enum

            // Simula AddValidationFailure
            _validationMock.Setup(v => v.AddValidationFailure(
                It.IsAny<string>(),
                It.IsAny<string>()
            )).Callback<string, string>((key, msg) =>
            {
                // opcional: puedes registrar los mensajes si quieres
            });

            // Simula ValidationException con diccionario
            var errores = new Dictionary<string, List<string>>
            {
                { nameof(request.CodigoProducto), new List<string> { "EL CÓDIGO DE PRODUCTO '999' NO EXISTE." } }
            };
            _validationMock.Setup(v => v.ValidationExceptionIfThereAreErrors())
                           .Throws(new ValidationException(errores));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
                await _handler.Handle(request, CancellationToken.None)
            );

            // Verifica que la excepción contiene la clave esperada
            Assert.True(ex.Errors.ContainsKey(nameof(request.CodigoProducto)));
            Assert.Contains("NO EXISTE", ex.Errors[nameof(request.CodigoProducto)].First());

            // Verifica que AddValidationFailure fue llamado
            _validationMock.Verify(v => v.AddValidationFailure(
                nameof(request.CodigoProducto),
                It.Is<string>(s => s.Contains("NO EXISTE"))
            ), Times.Once);
        }

        [Fact]
        public async Task Handle_NoProveedores_ThrowsNotFoundException()
        {
            // Arrange
            var request = new GetSuppliersListQuery("1234567890", "31");

            // Repositorio devuelve lista vacía de EFClienteAfiliacion
            _repositoryMock
                .Setup(r => r.GetSuppliersByAcceptor(It.IsAny<string>(), It.IsAny<int?>()))
                .ReturnsAsync(new List<EFClienteAfiliacion>());

            // Mapper nunca será llamado porque la lista está vacía
            // Validación no lanza excepción
            _validationMock.Setup(v => v.ValidationExceptionIfThereAreErrors());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<NotFoundException>(async () =>
                await _handler.Handle(request, CancellationToken.None)
            );

            Assert.Equal("NO HAY INFORMACIÓN RELACIONADA A LA CONSULTA", ex.Message);

            // Verifica que repositorio fue llamado
            _repositoryMock.Verify(r => r.GetSuppliersByAcceptor(It.IsAny<string>(), It.IsAny<int?>()), Times.Once);
        }


        [Fact]
        public async Task Handle_ProveedoresExistentes_RetornaListaMapeada()
        {
            // Arrange
            var request = new GetSuppliersListQuery("1234567890", "31");

            var efProveedores = new List<EFClienteAfiliacion>
            {
                new EFClienteAfiliacion { DocDuplicado = 1, NombreContacto1 = "Juan" }
            };

            _repositoryMock.Setup(r => r.GetSuppliersByAcceptor(It.IsAny<string>(), It.IsAny<int?>()))
                           .ReturnsAsync(efProveedores);

            _mapperMock.Setup(m => m.Map<List<ClienteAfiliacionVM>>(efProveedores))
                       .Returns(new List<ClienteAfiliacionVM>
                       {
                   new ClienteAfiliacionVM { docDuplicado = 1, nombreContacto1 = "Juan" }
                       });

            _validationMock.Setup(v => v.ValidationExceptionIfThereAreErrors()); // no falla

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Juan", result.First().nombreContacto1);
        }


        [Fact]
        public async Task Handle_ProveedoresVacio_ThrowsNotFoundException()
        {
            // Arrange
            var request = new GetSuppliersListQuery("1234567890", "31");

            _repositoryMock
                .Setup(r => r.GetSuppliersByAcceptor(It.IsAny<string>(), It.IsAny<int?>()))
                .ReturnsAsync(new List<EFClienteAfiliacion>()); // lista vacía

            _validationMock.Setup(v => v.ValidationExceptionIfThereAreErrors());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Equal("NO HAY INFORMACIÓN RELACIONADA A LA CONSULTA", ex.Message);
        }

        [Fact]
        public async Task Handle_ProveedoresExistentes_NoLanzaExcepcion()
        {
            // Arrange
            var request = new GetSuppliersListQuery("1234567890", "31");

            var proveedores = new List<EFClienteAfiliacion> { new EFClienteAfiliacion() };

            _repositoryMock
                .Setup(r => r.GetSuppliersByAcceptor(It.IsAny<string>(), It.IsAny<int?>()))
                .ReturnsAsync(proveedores);

            _mapperMock
                .Setup(m => m.Map<List<ClienteAfiliacionVM>>(It.IsAny<List<EFClienteAfiliacion>>()))
                .Returns(new List<ClienteAfiliacionVM> { new ClienteAfiliacionVM() });

            _validationMock.Setup(v => v.ValidationExceptionIfThereAreErrors());

            // Act
            var ex = await Record.ExceptionAsync(() => _handler.Handle(request, CancellationToken.None));

            // Assert
            Assert.Null(ex); // no lanza excepción
        }


    }

}
