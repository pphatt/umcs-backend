namespace Server.Application.Common.Dtos.Content.Media;

public class FileRequiredParamsDto
{
    public string type { get; set; } = default!;

    public Guid? userId { get; set; }

    public Guid? contributionId { get; set; }
}
