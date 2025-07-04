using RefactoringChallenge.Abstractions.Data;

namespace RefactoringChallenge.Data;

/// <summary>
/// Represents a time provider that always returns a fixed point in time.
/// </summary>
/// <remarks>
/// This class is useful for scenarios where a consistent and predictable time value is required,
/// such as in unit tests or simulations.
/// </remarks>
public class FixedTimeProvider(DateTime fixedNow) : ITimeProvider
{
    /// <summary>
    /// Gets the fixed point in time that this provider always returns.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the fixed time value.
    /// </value>
    /// <remarks>
    /// This property is useful for scenarios where a consistent and predictable time value is required,
    /// such as in unit tests or simulations.
    /// </remarks>
    public DateTime Now { get; } = fixedNow;
}