﻿using Server.Domain.Entity.Content;

namespace Server.Application.Common.Interfaces.Persistence.Repositories;

public interface IPrivateChatMessageRepository : IRepository<PrivateChatMessage, Guid>
{
}
