namespace LaunchBox.Core.Models;

/// <summary>
/// Represents a tag that can be applied to games
/// </summary>
public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }

    // Navigation properties
    public ICollection<Game> Games { get; set; } = new List<Game>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
