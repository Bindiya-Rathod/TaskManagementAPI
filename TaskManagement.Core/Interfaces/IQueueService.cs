namespace TaskManagement.Core.Interfaces
{
    public interface IQueueService
    {
        Task SendTaskAssignmentMessageAsync(object message);
    }
}
