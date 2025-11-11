namespace TaskManagement.Core.Exceptions
{
    public class TaskNotFoundException : Exception
    {
        public int TaskId { get; }

        public TaskNotFoundException(int taskId)
            : base($"Task with ID {taskId} was not found")
        {
            TaskId = taskId;
        }
    }
}
