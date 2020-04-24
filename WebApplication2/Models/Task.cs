using System;

namespace WebApplication2.Models
{
    public class Task
    {
        public int IdTask { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public string ProjectAssigned { get; set; }
        public string TaskType { get; set; }
        
    }
}