using FluentAssertions;
using Moq;
using Saga.Application.Enums;
using Saga.Application.Steps;
using Xunit;
using Saga.Application;

namespace Saga.UnitTests
{
    public class SagaOrchestratorTests
    {
        private readonly Mock<ISagaStep> _validateAndGetBookStepMock;
        private readonly Mock<ISagaStep> _initializeOrderStepMock;
        private readonly Mock<ISagaStep> _stockStepMock;
        private readonly Mock<ISagaStep> _completeOrderStepMock;
        private readonly SagaOrchestrator _sagaOrchestrator;

        public SagaOrchestratorTests()
        {
            _validateAndGetBookStepMock = new Mock<ISagaStep>();
            _initializeOrderStepMock = new Mock<ISagaStep>();
            _stockStepMock = new Mock<ISagaStep>();
            _completeOrderStepMock = new Mock<ISagaStep>();

            var steps = new List<ISagaStep>
            {
                _validateAndGetBookStepMock.Object,
                _initializeOrderStepMock.Object,
                _stockStepMock.Object,
                _completeOrderStepMock.Object
            };

            _sagaOrchestrator = new SagaOrchestrator(steps);
        }

        [Fact]
        public async Task ExecuteAsync_WhenAllStepsSucceed_ShouldCompleteSuccessfully()
        {
            // Arrange
            var context = new OrderSagaContext();
            var cancellationToken = CancellationToken.None;

            _validateAndGetBookStepMock.Setup(x => x.ExecuteAsync(It.IsAny<OrderSagaContext>(), cancellationToken))
                .ReturnsAsync(SagaStepResult.SuccessResult());
            _initializeOrderStepMock.Setup(x => x.ExecuteAsync(It.IsAny<OrderSagaContext>(), cancellationToken))
                .ReturnsAsync(SagaStepResult.SuccessResult());
            _stockStepMock.Setup(x => x.ExecuteAsync(It.IsAny<OrderSagaContext>(), cancellationToken))
                .ReturnsAsync(SagaStepResult.SuccessResult());
            _completeOrderStepMock.Setup(x => x.ExecuteAsync(It.IsAny<OrderSagaContext>(), cancellationToken))
                .ReturnsAsync(SagaStepResult.SuccessResult());

            // Act
            var result = await _sagaOrchestrator.ExecuteAsync(context, cancellationToken);

            // Assert
            result.Success.Should().BeTrue();
            
            _validateAndGetBookStepMock.Verify(x => x.ExecuteAsync(It.IsAny<OrderSagaContext>(), cancellationToken), Times.Once);
            _initializeOrderStepMock.Verify(x => x.ExecuteAsync(It.IsAny<OrderSagaContext>(), cancellationToken), Times.Once);
            _stockStepMock.Verify(x => x.ExecuteAsync(It.IsAny<OrderSagaContext>(), cancellationToken), Times.Once);
            _completeOrderStepMock.Verify(x => x.ExecuteAsync(It.IsAny<OrderSagaContext>(), cancellationToken), Times.Once);
           
            _validateAndGetBookStepMock.Verify(x => x.RollbackAsync(It.IsAny<OrderSagaContext>(), cancellationToken), Times.Never);
            _initializeOrderStepMock.Verify(x => x.RollbackAsync(It.IsAny<OrderSagaContext>(), cancellationToken), Times.Never);
            _stockStepMock.Verify(x => x.RollbackAsync(It.IsAny<OrderSagaContext>(), cancellationToken), Times.Never);
            _completeOrderStepMock.Verify(x => x.RollbackAsync(It.IsAny<OrderSagaContext>(), cancellationToken), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_WhenAnyStepFails_ShouldFailAndRollback()
        {
            // Arrange
            var context = new OrderSagaContext();
            var cancellationToken = CancellationToken.None;

            _validateAndGetBookStepMock.Setup(x => x.ExecuteAsync(It.IsAny<OrderSagaContext>(), cancellationToken))
                .ReturnsAsync(SagaStepResult.SuccessResult());
            _initializeOrderStepMock.Setup(x => x.ExecuteAsync(It.IsAny<OrderSagaContext>(), cancellationToken))
                .ReturnsAsync(SagaStepResult.Failed("Error in step 2", SagaErrorType.BadRequest));
            _stockStepMock.Setup(x => x.ExecuteAsync(It.IsAny<OrderSagaContext>(), cancellationToken))
                .ReturnsAsync(SagaStepResult.SuccessResult());
            _completeOrderStepMock.Setup(x => x.ExecuteAsync(It.IsAny<OrderSagaContext>(), cancellationToken))
                .ReturnsAsync(SagaStepResult.SuccessResult());

            // Act
            var result = await _sagaOrchestrator.ExecuteAsync(context, cancellationToken);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Error in step 2");
            result.ErrorType.Should().Be(SagaErrorType.BadRequest);

            _validateAndGetBookStepMock.Verify(x => x.ExecuteAsync(It.IsAny<OrderSagaContext>(), cancellationToken), Times.Once);
            _initializeOrderStepMock.Verify(x => x.ExecuteAsync(It.IsAny<OrderSagaContext>(), cancellationToken), Times.Once);
            _stockStepMock.Verify(x => x.ExecuteAsync(It.IsAny<OrderSagaContext>(), cancellationToken), Times.Never);
            _completeOrderStepMock.Verify(x => x.ExecuteAsync(It.IsAny<OrderSagaContext>(), cancellationToken), Times.Never);

            _validateAndGetBookStepMock.Verify(x => x.RollbackAsync(It.IsAny<OrderSagaContext>(), cancellationToken), Times.Never);
            _initializeOrderStepMock.Verify(x => x.RollbackAsync(It.IsAny<OrderSagaContext>(), cancellationToken), Times.Once);
            _stockStepMock.Verify(x => x.RollbackAsync(It.IsAny<OrderSagaContext>(), cancellationToken), Times.Never);
            _completeOrderStepMock.Verify(x => x.RollbackAsync(It.IsAny<OrderSagaContext>(), cancellationToken), Times.Never);
        }


        [Fact]
        public async Task ExecuteAsync_WhenFirstStepFails_ShouldCallRollbackOnlyOnce()
        {
            // Arrange
            var context = new OrderSagaContext();
            var cancellationToken = CancellationToken.None;

            _validateAndGetBookStepMock.Setup(x => x.ExecuteAsync(It.IsAny<OrderSagaContext>(), cancellationToken))
                .ReturnsAsync(SagaStepResult.Failed("Error in step 1", SagaErrorType.BadRequest));
            _initializeOrderStepMock.Setup(x => x.ExecuteAsync(It.IsAny<OrderSagaContext>(), cancellationToken))
                .ReturnsAsync(SagaStepResult.SuccessResult());
            _stockStepMock.Setup(x => x.ExecuteAsync(It.IsAny<OrderSagaContext>(), cancellationToken))
                .ReturnsAsync(SagaStepResult.SuccessResult());
            _completeOrderStepMock.Setup(x => x.ExecuteAsync(It.IsAny<OrderSagaContext>(), cancellationToken))
                .ReturnsAsync(SagaStepResult.SuccessResult());

            // Act
            var result = await _sagaOrchestrator.ExecuteAsync(context, cancellationToken);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Error in step 1");
            result.ErrorType.Should().Be(SagaErrorType.BadRequest);

            _validateAndGetBookStepMock.Verify(x => x.ExecuteAsync(It.IsAny<OrderSagaContext>(), cancellationToken), Times.Once);
            _initializeOrderStepMock.Verify(x => x.ExecuteAsync(It.IsAny<OrderSagaContext>(), cancellationToken), Times.Never);
            _stockStepMock.Verify(x => x.ExecuteAsync(It.IsAny<OrderSagaContext>(), cancellationToken), Times.Never);
            _completeOrderStepMock.Verify(x => x.ExecuteAsync(It.IsAny<OrderSagaContext>(), cancellationToken), Times.Never);
         
            _validateAndGetBookStepMock.Verify(x => x.RollbackAsync(It.IsAny<OrderSagaContext>(), cancellationToken), Times.Once);
            _initializeOrderStepMock.Verify(x => x.RollbackAsync(It.IsAny<OrderSagaContext>(), cancellationToken), Times.Never);
            _stockStepMock.Verify(x => x.RollbackAsync(It.IsAny<OrderSagaContext>(), cancellationToken), Times.Never);
            _completeOrderStepMock.Verify(x => x.RollbackAsync(It.IsAny<OrderSagaContext>(), cancellationToken), Times.Never);
        }
    }
}