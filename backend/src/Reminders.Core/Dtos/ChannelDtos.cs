using Reminders.Core.Utils;

namespace Reminders.Core.Dtos;

public class ChannelBindingResponse
{
    public string Link { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}

public class ChannelTargetResponse
{
    public Guid ChannelTargetId { get; set; }
    public ChannelType Type { get; set; }
    public string Title { get; set; } = null!;
    public string? ErrorMessage { get; set; }
}
