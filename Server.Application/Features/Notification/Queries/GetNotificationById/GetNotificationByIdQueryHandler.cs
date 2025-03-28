using AutoMapper;

using ErrorOr;

using MediatR;

using Server.Application.Common.Dtos.Content.Notification;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;

namespace Server.Application.Features.Notification.Queries.GetNotificationById;

public class GetNotificationByIdQueryHandler : IRequestHandler<GetNotificationByIdQuery, ErrorOr<ResponseWrapper<NotificationDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetNotificationByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ErrorOr<ResponseWrapper<NotificationDto>>> Handle(GetNotificationByIdQuery request, CancellationToken cancellationToken)
    {
        var notification = await _unitOfWork.NotificationRepository.GetByIdAsync(request.Id);

        if (notification is null)
        {
            return Errors.Notification.CannotFound;
        }

        var result = _mapper.Map<NotificationDto>(notification);

        return new ResponseWrapper<NotificationDto>
        {
            IsSuccessful = true,
            ResponseData = result
        };
    }
}
