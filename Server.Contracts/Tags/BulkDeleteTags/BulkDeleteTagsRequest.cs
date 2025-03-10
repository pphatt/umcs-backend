namespace Server.Contracts.Tags.BulkDeleteTags;

public class BulkDeleteTagsRequest
{
    public List<Guid> TagIds { get; set; } = default!;
}
