namespace Server.Application.Common.Interfaces.Persistence;

public interface IUnitOfWork
{
    Task<int> CompleteAsync();
}
