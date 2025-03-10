using FluentValidation;

namespace Server.Application.Features.TagApp.Commands.BulkDeleteTags;

public class BulkDeleteTagsCommandValidator : AbstractValidator<BulkDeleteTagsCommand>
{
    public BulkDeleteTagsCommandValidator()
    {
    }
}
