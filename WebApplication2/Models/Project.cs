using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace WebApplication2.Models
{
    public class Project
    {
        public int IdProject { get; set; }
        public string Name { get; set; }
        public DateTime Deadline { get; set; }
        public List<Task> AssociatedTasks { get; set; }
    }
}