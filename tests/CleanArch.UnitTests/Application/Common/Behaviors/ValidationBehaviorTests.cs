using CleanArch.Application.Common.Behaviors;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using NSubstitute;

namespace CleanArch.UnitTests.Application.Common.Behaviors;

public record TestRequest(string Name) : IRequest<string>;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_WhenNoValidatorsExist_ShouldCallNextDelegate()
    {
        // Arrange
        var validators = Enumerable.Empty<IValidator<TestRequest>>();
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var request = new TestRequest("Sample");
        var nextDelegate = Substitute.For<RequestHandlerDelegate<string>>();
        nextDelegate.Invoke().Returns(Task.FromResult("Success"));

        // Act
        var result = await behavior.Handle(request, nextDelegate, CancellationToken.None);

        // Assert
        result.Should().Be("Success");
        await nextDelegate.Received(1).Invoke();
    }

    [Fact]
    public async Task Handle_WhenValidationPasses_ShouldCallNextDelegate()
    {
        // Arrange
        var validator = Substitute.For<IValidator<TestRequest>>();
        validator.ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));

        var behavior = new ValidationBehavior<TestRequest, string>(new[] { validator });
        var request = new TestRequest("Valid Name");
        var nextDelegate = Substitute.For<RequestHandlerDelegate<string>>();
        nextDelegate.Invoke().Returns(Task.FromResult("Success"));

        // Act
        var result = await behavior.Handle(request, nextDelegate, CancellationToken.None);

        // Assert
        result.Should().Be("Success");
        await nextDelegate.Received(1).Invoke();
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldThrowValidationException()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new("Name", "Name is required.")
        };
        var validator = Substitute.For<IValidator<TestRequest>>();
        validator.ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult(failures)));

        var behavior = new ValidationBehavior<TestRequest, string>(new[] { validator });
        var request = new TestRequest("");
        var nextDelegate = Substitute.For<RequestHandlerDelegate<string>>();

        // Act
        Func<Task> act = () => behavior.Handle(request, nextDelegate, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<ValidationException>();
        exception.Which.Errors.Should().ContainSingle(e => e.PropertyName == "Name" && e.ErrorMessage == "Name is required.");
        await nextDelegate.DidNotReceive().Invoke();
    }
}
