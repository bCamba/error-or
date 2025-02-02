using ErrorOr;
using FluentAssertions;

namespace Tests;

public class SwitchAsyncTests
{
    private record Person(string Name);

    [Fact]
    public async Task CallingSwitchAsync_WhenIsSuccess_ShouldExecuteOnValueAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Amichai");
        Task OnValueAction(Person person) => Task.FromResult(person.Should().BeEquivalentTo(errorOrPerson.Value));
        Task OnErrorsAction(IReadOnlyList<Error> _) => throw new Exception("Should not be called");

        // Act
        Func<Task> action = async () => await errorOrPerson.SwitchAsync(
            OnValueAction,
            OnErrorsAction);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task CallingSwitchAsync_WhenIsError_ShouldExecuteOnErrorAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new List<Error> { Error.Validation(), Error.Conflict() };
        Task OnValueAction(Person _) => throw new Exception("Should not be called");
        Task OnErrorsAction(IReadOnlyList<Error> errors) => Task.FromResult(errors.Should().BeEquivalentTo(errorOrPerson.Errors));

        // Act
        Func<Task> action = async () => await errorOrPerson.SwitchAsync(
            OnValueAction,
            OnErrorsAction);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task CallingSwitchFirstAsync_WhenIsSuccess_ShouldExecuteOnValueAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Amichai");
        Task OnValueAction(Person person) => Task.FromResult(person.Should().BeEquivalentTo(errorOrPerson.Value));
        Task OnFirstErrorAction(Error _) => throw new Exception("Should not be called");

        // Act
        Func<Task> action = async () => await errorOrPerson.SwitchFirstAsync(
            OnValueAction,
            OnFirstErrorAction);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task CallingSwitchFirstAsync_WhenIsError_ShouldExecuteOnFirstErrorAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new List<Error> { Error.Validation(), Error.Conflict() };
        Task OnValueAction(Person _) => throw new Exception("Should not be called");
        Task OnFirstErrorAction(Error errors)
            => Task.FromResult(errors.Should().BeEquivalentTo(errorOrPerson.Errors[0])
                .And.BeEquivalentTo(errorOrPerson.FirstError));

        // Act
        Func<Task> action = async () => await errorOrPerson.SwitchFirstAsync(
            OnValueAction,
            OnFirstErrorAction);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task CallingSwitchFirstAsyncAfterThenAsync_WhenIsSuccess_ShouldExecuteOnValueAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Amichai");
        Task OnValueAction(Person person) => Task.FromResult(person.Should().BeEquivalentTo(errorOrPerson.Value));
        Task OnFirstErrorAction(Error _) => throw new Exception("Should not be called");

        // Act
        Func<Task> action = async () => await errorOrPerson
            .ThenAsync(person => Task.FromResult(person))
            .SwitchFirstAsync(
                OnValueAction,
                OnFirstErrorAction);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task CallingSwitchAsyncAfterThenAsync_WhenIsSuccess_ShouldExecuteOnValueAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Amichai");
        Task OnValueAction(Person person) => Task.FromResult(person.Should().BeEquivalentTo(errorOrPerson.Value));
        Task OnErrorsAction(IReadOnlyList<Error> _) => throw new Exception("Should not be called");

        // Act
        Func<Task> action = async () => await errorOrPerson
            .ThenAsync(person => Task.FromResult(person))
            .SwitchAsync(OnValueAction, OnErrorsAction);

        // Assert
        await action.Should().NotThrowAsync();
    }
}
