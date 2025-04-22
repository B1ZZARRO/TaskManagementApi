using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Data;
using TaskManagementApi.Data.Models;
using TaskManagementApi.ViewModels;

namespace TaskManagementApi.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly DatabaseContext _context;
        
        public TaskController(DatabaseContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Получение задач пользователя
        /// </summary>
        [Route("tasks/user/{userId}")]
        [HttpGet]
        public IActionResult GetUserTasks(int userId)
        {
            try
            {
                var tasks = _context.TaskAssignments
                    .Where(ta => ta.AssignedUserId == userId)
                    .Include(ta => ta.Task)
                    .ThenInclude(t => t.TaskPriority)
                    .Include(ta => ta.Task)
                    .ThenInclude(t => t.TaskStatus)
                    .Select(ta => new TaskData
                    {
                        TaskId = ta.Task.TaskId,
                        Title = ta.Task.Title,
                        Description = ta.Task.Description,
                        CreatedAt = ta.Task.CreatedAt,
                        Deadline = ta.Task.Deadline,
                        PriorityName = ta.Task.TaskPriority.PriorityName,
                        StatusName = ta.Task.TaskStatus.StatusName,
                        TaskCreatorId = ta.Task.TaskCreatorId,
                        AssignedUserId = ta.AssignedUserId
                    })
                    .ToList();

                return Ok(new ApiResponse<IEnumerable<TaskData>>(tasks));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }

        /// <summary>
        /// Получение всех задач
        /// </summary>
        [Route("tasks/all")]
        [HttpGet]
        public IActionResult GetAllTasks()
        {
            try
            {
                var tasks = _context.Tasks
                    .Include(t => t.TaskPriority)
                    .Include(t => t.TaskStatus)
                    .Include(t => t.TaskCreator)
                    .Select(t => new TaskData
                    {
                        TaskId = t.TaskId,
                        Title = t.Title,
                        Description = t.Description,
                        CreatedAt = t.CreatedAt,
                        Deadline = t.Deadline,
                        PriorityName = t.TaskPriority.PriorityName,
                        StatusName = t.TaskStatus.StatusName,
                        TaskCreatorId = t.TaskCreatorId,
                        TaskCreatorName = $"{t.TaskCreator.Name} {t.TaskCreator.LastName}"
                    })
                    .ToList();
                
                return Ok(new ApiResponse<IEnumerable<TaskData>>(tasks));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Получение суммарной информации о задачах, связанных с пользователями и группами.
        /// </summary>
        [HttpGet("GetTaskSummary")]
        public IActionResult GetTaskSummary()
        {
            try
            {
                var taskSummary = _context.TaskAssignments
                    .Include(ta => ta.Task)
                    .Include(ta => ta.AssignedUser)
                    .Include(ta => ta.AssignedGroup)
                    .Select(ta => new TaskAssignmentData1
                    {
                        TaskId = ta.Task.TaskId,
                        TaskTitle = ta.Task.Title,
                        TaskDescription = ta.Task.Description,
                        CreatedAt = ta.Task.CreatedAt,
                        Deadline = ta.Task.Deadline,
                        CompletionDate = ta.Task.CompletionDate,
                        ResultsNotes = ta.Task.ResultsNotes,
                        UserId = ta.AssignedUser.UserId,
                        FullName = $"{ta.AssignedUser.LastName} {ta.AssignedUser.Name} {ta.AssignedUser.Surname}",
                        GroupId = ta.AssignedGroup.GroupId,
                        GroupName = ta.AssignedGroup.GroupName
                    })
                    .ToList();
                
                return Ok(new ApiResponse<IEnumerable<TaskAssignmentData1>>(taskSummary));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Добавление новой задачи
        /// </summary>
        [Route("task/add")]
        [HttpPost]
        public IActionResult AddTask(TaskCreationData taskData)
        {
            try
            {
                var newTask = new Task
                {
                    Title = taskData.Title,
                    Description = taskData.Description,
                    CreatedAt = DateTime.Now,
                    Deadline = taskData.Deadline,
                    PriorityId = taskData.PriorityId,
                    StatusId = taskData.StatusId,
                    TaskCreatorId = taskData.TaskCreatorId
                };

                _context.Tasks.Add(newTask);
                _context.SaveChanges();

                return Ok(new ApiResponseMessage("Задача успешно добавлена"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }

        /// <summary>
        /// Присваивание задачи пользователю или группе
        /// </summary>
        [Route("task/assign")]
        [HttpPost]
        public IActionResult AssignTask(TaskAssignmentData assignmentData)
        {
            try
            {
                var task = _context.Tasks.FirstOrDefault(t => t.TaskId == assignmentData.TaskId);
                if (task == null)
                    return NotFound(new ApiResponseMessage("Задача не найдена"));

                // Назначение задачи пользователю
                if (assignmentData.AssignedUserId.HasValue)
                {
                    var user = _context.Users.FirstOrDefault(u => u.UserId == assignmentData.AssignedUserId.Value);
                    if (user == null)
                        return NotFound(new ApiResponseMessage("Пользователь не найден"));

                    _context.TaskAssignments.Add(new TaskAssignment
                    {
                        TaskId = task.TaskId,
                        AssignedUserId = user.UserId
                    });
                }

                // Назначение задачи группе
                if (assignmentData.AssignedGroupId.HasValue)
                {
                    var group = _context.UserGroups.FirstOrDefault(g => g.GroupId == assignmentData.AssignedGroupId.Value);
                    if (group == null)
                        return NotFound(new ApiResponseMessage("Группа не найдена"));

                    _context.TaskAssignments.Add(new TaskAssignment
                    {
                        TaskId = task.TaskId,
                        AssignedGroupId = group.GroupId
                    });
                }

                _context.SaveChanges();
                return Ok(new ApiResponseMessage("Задача успешно назначена"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Получение списка задач, назначенных пользователям определенной группы
        /// </summary>
        [Route("tasks/by-group")]
        [HttpGet]
        public IActionResult GetTasksByGroupId(int groupId)
        {
            try
            {
                // Проверка существования группы
                var group = _context.UserGroups.FirstOrDefault(g => g.GroupId == groupId);
                if (group == null)
                    return NotFound(new ApiResponseMessage("Группа не найдена"));

                // Получение задач, назначенных пользователям данной группы
                var tasks = _context.TaskAssignments
                    .Include(ta => ta.Task)
                    .Include(ta => ta.AssignedUser)
                    .Where(ta => ta.AssignedGroupId == groupId)
                    .Select(ta => new TaskData
                    {
                        TaskId = ta.Task.TaskId,
                        Title = ta.Task.Title,
                        Description = ta.Task.Description,
                        CreatedAt = ta.Task.CreatedAt,
                        Deadline = ta.Task.Deadline,
                        PriorityId = ta.Task.PriorityId,
                        PriorityName = ta.Task.TaskPriority.PriorityName,
                        StatusId = ta.Task.StatusId,
                        StatusName = ta.Task.TaskStatus.StatusName,
                        TaskCreatorId = ta.Task.TaskCreatorId,
                        TaskCreatorName = _context.Users
                                                 .Where(u => u.UserId == ta.Task.TaskCreatorId)
                                                 .Select(u => u.Name)
                                                 .FirstOrDefault(),
                        AssignedUserId = ta.AssignedUserId,
                        AssignedUserName = ta.AssignedUser != null 
                                          ? $"{ta.AssignedUser.Name} {ta.AssignedUser.Surname}" 
                                          : null,
                        AssignedGroupId = groupId,
                        GroupName = group.GroupName
                    })
                    .ToList();

                if (!tasks.Any())
                    return NotFound(new ApiResponseMessage("Для данной группы задачи не найдены"));

                return Ok(new ApiResponse<List<TaskData>>(tasks));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Изменение задачи
        /// </summary>
        [Route("update-task/{taskId}")]
        [HttpPut]
        public IActionResult UpdateTask(int taskId, [FromBody] UpdateTaskData model)
        {
            try
            {
                var task = _context.Tasks.FirstOrDefault(t => t.TaskId == taskId);
                if (task == null)
                    return NotFound(new ApiResponseMessage("Задача не найдена"));

                task.Title = model.Title ?? task.Title;
                task.Description = model.Description ?? task.Description;
                task.Deadline = model.Deadline ?? task.Deadline;
                task.PriorityId = model.PriorityId ?? task.PriorityId;
                task.StatusId = model.StatusId ?? task.StatusId;
                task.CompletionDate = model.CompletionDate ?? task.CompletionDate;
                task.ResultsNotes = model.ResultsNotes ?? task.ResultsNotes;

                _context.SaveChanges();
                return Ok(new ApiResponseMessage("Задача успешно обновлена"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Удаление задачи
        /// </summary>
        [Route("delete-task/{taskId}")]
        [HttpDelete]
        public IActionResult DeleteTask(int taskId)
        {
            try
            {
                var task = _context.Tasks.FirstOrDefault(t => t.TaskId == taskId);
                if (task == null)
                    return NotFound(new ApiResponseMessage("Задача не найдена"));

                _context.Tasks.Remove(task);
                _context.SaveChanges();
                return Ok(new ApiResponseMessage("Задача успешно удалена"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
    }
}