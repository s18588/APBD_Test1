using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApplication2.Models;

namespace WebApplication2.Responses
{
    public class TaskResponse
    {    
        [Required]
        public TeamMember TeamMember { get; set; }
        [Required]
        public List<Task> TasksCreated { get; set; }
        [Required]
        public List<Task> TasksAssignedTo { get; set; }
    }
}