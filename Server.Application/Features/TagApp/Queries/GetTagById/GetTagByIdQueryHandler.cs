using AutoMapper;
using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Content.Tag;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;

namespace Server.Application.Features.TagApp.Queries.GetTagById;

public class GetTagByIdQueryHandler : IRequestHandler<GetTagByIdQuery, ErrorOr<ResponseWrapper<TagDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTagByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ErrorOr<ResponseWrapper<TagDto>>> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
    {
        var tag = await _unitOfWork.TagRepository.GetByIdAsync(request.Id);

        if (tag is null)
        {
            return Errors.Tags.CannotFound;
        }

        var result = _mapper.Map<TagDto>(tag);

        return new ResponseWrapper<TagDto>
        {
            IsSuccessful = true,
            ResponseData = result
        };
    }
}
