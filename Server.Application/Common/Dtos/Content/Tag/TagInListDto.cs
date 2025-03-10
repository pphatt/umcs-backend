namespace Server.Application.Common.Dtos.Content.Tag;

public class TagInListDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public DateTime? DateDeleted { get; set; }
}
