namespace Server.Contracts.Notifications.BulkDeleteNotifications;

public class BulkDeleteNotificationsRequest
{
    public List<Guid> Ids { get; set; }
}
