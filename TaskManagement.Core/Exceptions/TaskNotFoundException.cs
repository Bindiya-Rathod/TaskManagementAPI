namespace TaskManagement.Core.Exceptions
{
    public class TaskNotFoundException : Exception
    {
        public int TaskId { get; }
        public TaskNotFoundException(string message) : base(message)
        {
        }
        public TaskNotFoundException(int taskId)
            : base($"Task with ID {taskId} was not found")
        {
            TaskId = taskId;
        }

        public TaskNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
