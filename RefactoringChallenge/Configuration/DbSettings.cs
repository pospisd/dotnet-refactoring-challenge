namespace RefactoringChallenge.Configuration;

/// <summary>
/// Represents the database settings configuration, including the connection string.
/// </summary>
internal sealed class DbSettings
{
    /// <summary>
    /// Gets or sets the connection string used to connect to the database.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> representing the database connection string.
    /// </value>
    public string ConnectionString { get; set; } = string.Empty;
}