using FluentValidation;
using Interbank.Productos.Comercial.Fcd.Application.Behaviours;
using MediatR;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Behaviours
{
    public class ValidationBehaviourTests
    {
        private record FakeRequest(string? Name = null) : IRequest<object>;

        [Fact]
        public void Constructor_Should_Create_Instance()
        {
            var validators = Array.Empty<IValidator<FakeRequest>>();
            var behaviour = new ValidationBehaviour<FakeRequest, object>(validators);
            Assert.NotNull(behaviour);
        }

        [Fact]
        public async Task Handle_Should_Invoke_Next_When_No_Validators()
        {
            // Arrange
            var validators = Array.Empty<IValidator<FakeRequest>>();
            var behaviour = new ValidationBehaviour<FakeRequest, object>(validators);

            var request = new FakeRequest();

            var nextCalled = false;
            RequestHandlerDelegate<object> next = _ =>
            {
                nextCalled = true;
                return Task.FromResult<object>(new());
            };

            // Act
            var result = await behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            Assert.True(nextCalled);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Handle_Should_Invoke_Next_When_Validation_Passes()
        {
            // Arrange
            var validator = new InlineValidator<FakeRequest>();
            var validators = new[] { validator };

            var behaviour = new ValidationBehaviour<FakeRequest, object>(validators);
            var request = new FakeRequest();

            var nextCalled = false;
            RequestHandlerDelegate<object> next = _ =>
            {
                nextCalled = true;
                return Task.FromResult<object>(new());
            };

            // Act
            var result = await behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            Assert.True(nextCalled);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Handle_Should_Throw_ValidationException_When_Validation_Fails()
        {

            // Arrange
            var validator = new InlineValidator<FakeRequest>();
            validator.RuleFor(x => x.Name).NotNull();

            var behaviour = new ValidationBehaviour<FakeRequest, object>(new[] { validator });
            var request = new FakeRequest(); // Name = null → fuerza error

            RequestHandlerDelegate<object> next = _ => Task.FromResult<object>(new());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exceptions.ValidationException>(() =>
                behaviour.Handle(request, next, CancellationToken.None));

            // Verificamos que la excepción contiene errores
            Assert.Single(ex.Errors); // solo Name debería fallar
            Assert.True(ex.Errors.ContainsKey("Name"));
            Assert.Contains("Name", ex.Errors.Keys);
        }
    }
}
