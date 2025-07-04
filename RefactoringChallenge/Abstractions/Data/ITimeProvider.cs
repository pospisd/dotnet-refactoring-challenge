namespace RefactoringChallenge.Abstractions.Data;

/// <summary>
/// Provides an abstraction for retrieving the current date and time.
/// </summary>
/// <remarks>
/// This interface is useful for decoupling time-dependent logic from system time,
/// enabling easier testing and flexibility in time-based operations.
/// </remarks>
public interface ITimeProvider
{
    /// <summary>
    /// Gets the current date and time.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the current date and time.
    /// </value>
    /// <remarks>
    /// This property provides an abstraction for retrieving the current date and time,
    /// allowing for easier testing and flexibility in time-dependent logic.
    /// </remarks>
    DateTime Now { get; }
}