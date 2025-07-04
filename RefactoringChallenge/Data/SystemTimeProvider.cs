using RefactoringChallenge.Abstractions.Data;

namespace RefactoringChallenge.Data;

/// <summary>
/// Provides the current system date and time.
/// </summary>
/// <remarks>
/// This class implements the <see cref="RefactoringChallenge.Abstractions.Data.ITimeProvider"/> interface
/// and retrieves the current date and time using the system clock.
/// </remarks>
internal class SystemTimeProvider : ITimeProvider
{
    /// <summary>
    /// Gets the current date and time.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the current date and time.
    /// </value>
    /// <remarks>
    /// This property retrieves the system's current date and time using <see cref="DateTime.Now"/>.
    /// It is useful for scenarios where real-time data is required.
    /// </remarks>
    public DateTime Now => DateTime.Now;
}