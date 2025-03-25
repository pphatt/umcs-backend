namespace Server.Application.Common.Dtos.Content.Notification;

public class NotificationDto
{
    public string Title { get; set; } = default!;

    public string Content { get; set; } = default!;

    public string Username { get; set; } = default!;

    public string Avatar { get; set; } = default!;

    public string Type { get; set; } = default!;

    public bool HasRed { get; set; }

    public DateTime DateCreated { get; set; }
}
