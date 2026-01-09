using System;
using System.Collections.Generic;

namespace MyWinFormsApp
{
    public enum TaskStatus
    {
        Deleted = -1,
        Todo = 0,
        InProgress = 1,
        Done = 2
    }

    public enum TaskPriority
    {
        Low = 0,
        Medium = 1,
        High = 2
    }

    public class TaskMoveHistory
    {
        public TaskStatus FromStatus { get; set; }
        public TaskStatus ToStatus { get; set; }
        public DateTime MovedDate { get; set; }
    }

    public class TaskItem
    {
        public string Id { get; set; }
        public int TaskNumber { get; set; }
        public string Title { get; set; }
        public string Assignee { get; set; } = string.Empty;
        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public DateTime CreatedDate { get; set; }
        public List<TaskMoveHistory> MoveHistory { get; set; }

        public TaskItem()
        {
            Id = Guid.NewGuid().ToString();
            Title = string.Empty;
            Status = TaskStatus.Todo;
            Priority = TaskPriority.Medium;
            CreatedDate = DateTime.Now;
            MoveHistory = new List<TaskMoveHistory>();
        }

        public TaskItem(string title)
        {
            Id = Guid.NewGuid().ToString();
            Title = title;
            Status = TaskStatus.Todo;
            Priority = TaskPriority.Medium;
            CreatedDate = DateTime.Now;
            MoveHistory = new List<TaskMoveHistory>();
        }
    }
}
