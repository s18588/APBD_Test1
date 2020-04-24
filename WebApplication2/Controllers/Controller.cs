using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Responses;
using System.Data.SqlClient;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class Controller : ControllerBase
    {
        private string connstring =
            "Data Source=10.1.1.36,1433;Initial Catalog=s18588;User ID=apbds18588;Password=admin";
        
        [HttpGet("{id}")]
        public IActionResult getTeamMemberInfo(int id)
        {
            var response = new TaskResponse();
            response.TasksCreated = new List<Task>();
            response.TasksAssignedTo = new List<Task>();
            var cl = new HttpClient();
            cl.BaseAddress = new Uri("http://localhost:5001/api/tasks");
            
            using (var c = new SqlConnection(connstring))
            using (var com = new SqlCommand())
            {
                com.Connection = c;
                c.Open();
                
                // Check if team member exists
                com.CommandText =
                    "Select * from TeamMember where IdTeamMember = @id";
                com.Parameters.AddWithValue("id", id);
                var dr = com.ExecuteReader();
                if (!dr.Read())
                {
                    dr.Close();
                    return BadRequest("Member does not exist!");
                }
                var TeamMember = new TeamMember();
                TeamMember.IdTeamMember = (int) dr[0];
                TeamMember.FirstName = dr[1] as string;
                TeamMember.LastName = dr[2] as string;
                TeamMember.Email = dr[3] as string;
                response.TeamMember = TeamMember;
                com.Parameters.Clear();
                dr.Close();
                Console.WriteLine("Member created.");
                
                // Get tasks assigned to
                com.CommandText =
                    "select IdTask,Task.Name,Task.Description,Task.Deadline,P.Name,TT.Name from Task join TaskType TT on Task.IdTaskType = TT.IdTaskType join Project P on Task.IdProject = P.IdProject where IdAssignedTo = @id;";
                com.Parameters.AddWithValue("id", id);
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var Task = new Task();
                    Task.IdTask = (int) dr[0];
                    Task.Name = dr[1].ToString();
                    Task.Description = dr[2] as string;
                    Task.Deadline = (DateTime) dr[3];
                    Task.ProjectAssigned = dr[4] as string;
                    Task.TaskType = dr[5] as string;
                    response.TasksAssignedTo.Add(Task);
                }
                com.Parameters.Clear();
                dr.Close();
                
                // Get Tasks created 
                
                com.CommandText =
                    "select IdTask,Task.Name,Task.Description,Task.Deadline,P.Name,TT.Name from Task join TaskType TT on Task.IdTaskType = TT.IdTaskType join Project P on Task.IdProject = P.IdProject where IdCreator = @id;";
                com.Parameters.AddWithValue("id", id);
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var Task = new Task();
                    Task.IdTask = (int) dr[0];
                    Task.Name = dr[1].ToString();
                    Task.Description = dr[2] as string;
                    Task.Deadline = (DateTime) dr[3];
                    Task.ProjectAssigned = dr[4] as string;
                    Task.TaskType = dr[5] as string;
                    response.TasksCreated.Add(Task);
                }


            }
            return Ok(response);
        }


        [HttpDelete("{id}")]
        public IActionResult deleteInfoAboutProject(int id)
        {
            var response = new ProjectDeleteResponse();
            var Project = new Project();
            Project.AssociatedTasks = new List<Task>();
            var cl = new HttpClient();
            cl.BaseAddress = new Uri("http://localhost:5001/api/tasks");
            using (var c = new SqlConnection(connstring))
            using (var com = new SqlCommand())
            {
                com.Connection = c;
                c.Open();
                var transaction = c.BeginTransaction();
                com.Transaction = transaction;

                // check if project exists
                com.CommandText =
                    "Select IdProject,Name,Deadline from Project where IdProject = @id";
                com.Parameters.AddWithValue("id", id);
                var dr = com.ExecuteReader();
                if (!dr.Read())
                {
                    dr.Close();
                    transaction.Rollback();
                    return BadRequest("Given project does not exist!");
                }

                Project.IdProject = (int) dr[0];
                Project.Name = dr[1].ToString();
                Project.Deadline = (DateTime) dr[2];

                com.Parameters.Clear();
                dr.Close();
                // add associated tasks to response
                com.CommandText =
                    "select IdTask,Task.Name,Description,Deadline,TT.Name from Task join TaskType TT on Task.IdTaskType = TT.IdTaskType where IdProject =@id;";
                com.Parameters.AddWithValue("id", id);
                dr = com.ExecuteReader();

                while (dr.Read())
                {
                    var Task = new Task();
                    Task.IdTask = (int) dr[0];
                    Task.Name = dr[1].ToString();
                    Task.Description = dr[2] as string;
                    Task.Deadline = (DateTime) dr[3];
                    Task.TaskType = dr[4] as string;
                    Project.AssociatedTasks.Add(Task);
                }

                com.Parameters.Clear();
                dr.Close();

                com.CommandText =
                    "delete from Task where IdProject =@id;";
                com.Parameters.AddWithValue("id", id);
                com.ExecuteNonQuery();
                
                com.Parameters.Clear();
                dr.Close();

                com.CommandText =
                    "delete from Project where IdProject =@id;";
                com.Parameters.AddWithValue("id", id);
                com.ExecuteNonQuery();
                transaction.Commit();
                
                
                response.Project = Project;
            }
            
            return Ok(response);
        }
        
    }
    
}