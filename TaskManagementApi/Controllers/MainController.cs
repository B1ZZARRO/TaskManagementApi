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
    public class MainController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public MainController(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Регистрация
        /// </summary>
        [Route("reg")]
        [HttpPost]
        public IActionResult Reg(UserData userData)
        {
            try
            {
                // Проверка, существует ли пользователь с таким логином
                if (_context.Users.Any(u => u.Login == userData.Login))
                    return BadRequest(new ApiResponseMessage("Пользователь с таким логином уже существует"));

                var newUser = new User
                {
                    LastName = userData.LastName,
                    Name = userData.Name,
                    Surname = userData.Surname,
                    Login = userData.Login,
                    Password = userData.Password,
                    RoleId = userData.RoleId,
                    GroupId = userData.GroupId
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                return Ok(new ApiResponseMessage("Пользователь успешно зарегистрирован"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }

        /// <summary>
        /// Авторизация
        /// </summary>
        [Route("auth")]
        [HttpPost]
        public IActionResult Auth(UserAuthData authData)
        {
            try
            {
                var user = _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.UserGroup)
                    .FirstOrDefault(u => u.Login == authData.Login && u.Password == authData.Password);

                if (user == null)
                    return Unauthorized(new ApiResponseMessage("Неверный логин или пароль"));

                var userData = new UserData
                {
                    UserId = user.UserId,
                    LastName = user.LastName,
                    Name = user.Name,
                    Surname = user.Surname,
                    Login = user.Login,
                    RoleId = user.RoleId,
                    RoleName = user.Role?.RoleName,
                    GroupId = user.GroupId,
                    GroupName = user.UserGroup?.GroupName
                };

                return Ok(new ApiResponse<UserData>(userData));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
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
                        /*TaskId = ta.Task.TaskId,
                        TaskTitle = ta.Task.Title,
                        TaskDescription = ta.Task.Description,
                        CreatedAt = ta.Task.CreatedAt,
                        Deadline = ta.Task.Deadline,
                        CompletionDate = ta.Task.CompletionDate,
                        ResultsNotes = ta.Task.ResultsNotes,
                        AssignedUser = ta.AssignedUser != null
                            ? new UserData1()
                            {
                                UserId = ta.AssignedUser.UserId,
                                FullName = $"{ta.AssignedUser.LastName} {ta.AssignedUser.Name} {ta.AssignedUser.Surname}"
                            }
                            : null,
                        AssignedGroup = ta.AssignedGroup != null
                            ? new GroupData1()
                            {
                                GroupId = ta.AssignedGroup.GroupId,
                                GroupName = ta.AssignedGroup.GroupName
                            }
                            : null*/
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
        /// Поиск пользователя по логину
        /// </summary>
        [Route("user/search")]
        [HttpGet]
        public IActionResult SearchUserByLogin(string login)
        {
            try
            {
                var user = _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.UserGroup)
                    .FirstOrDefault(u => u.Login == login);

                if (user == null)
                    return NotFound(new ApiResponseMessage("Пользователь не найден"));
                
                var userData = new UserData
                {
                    UserId = user.UserId,
                    LastName = user.LastName,
                    Name = user.Name,
                    Surname = user.Surname,
                    Login = user.Login,
                    RoleId = user.RoleId,
                    RoleName = user.Role?.RoleName,
                    GroupId = user.GroupId,
                    GroupName = user.UserGroup?.GroupName
                };
                
                return Ok(new ApiResponse<UserData>(userData));
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
        /// Получение списка пользователей по номеру группы
        /// </summary>
        /// <param name="groupId">Идентификатор группы</param>
        /// <returns>Список пользователей</returns>
        [Route("users/by-group")]
        [HttpGet]
        public IActionResult GetUsersByGroupId(int groupId)
        {
            try
            {
                var group = _context.UserGroups.FirstOrDefault(g => g.GroupId == groupId);
                if (group == null)
                    return NotFound(new ApiResponseMessage("Группа не найдена"));

                var users = _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.UserGroup)
                    .Where(u => u.GroupId == groupId)
                    .Select(u => new UserData
                    {
                        UserId = u.UserId,
                        LastName = u.LastName,
                        Name = u.Name,
                        Surname = u.Surname,
                        Login = u.Login,
                        Password = u.Password,
                        RoleId = u.RoleId,
                        RoleName = u.Role.RoleName,
                        GroupId = u.GroupId,
                        GroupName = u.UserGroup.GroupName
                    })
                    .ToList();

                if (!users.Any())
                    return NotFound(new ApiResponseMessage("В данной группе нет пользователей"));

                return Ok(new ApiResponse<List<UserData>>(users));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Получение списка задач, назначенных пользователям определенной группы
        /// </summary>
        /// <param name="groupId">Идентификатор группы</param>
        /// <returns>Список задач</returns>
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
        /// Получение списка всех пользователей.
        /// </summary>
        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.UserGroup)
                    .Select(u => new UserData
                    {
                        UserId = u.UserId,
                        LastName = u.LastName,
                        Name = u.Name,
                        Surname = u.Surname,
                        Login = u.Login,
                        Password = u.Password,
                        RoleId = u.RoleId,
                        RoleName = u.Role.RoleName,
                        GroupId = u.GroupId,
                        GroupName = u.UserGroup.GroupName
                    })
                    .ToList();

                return Ok(new ApiResponse<List<UserData>>(users));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
            
        }

        /// <summary>
        /// Получение списка всех ролей.
        /// </summary>
        [HttpGet("GetAllRoles")]
        public IActionResult GetAllRoles()
        {
            try
            {
                var roles = _context.Roles
                    .Select(r => new RoleData
                    {
                        RoleId = r.RoleId,
                        RoleName = r.RoleName
                    })
                    .ToList();

                return Ok(new ApiResponse<List<RoleData>>(roles));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }

        /// <summary>
        /// Получение списка всех групп.
        /// </summary>
        [HttpGet("GetAllGroups")]
        public IActionResult GetAllGroups()
        {
            try
            {
                var groups = _context.UserGroups
                    .Select(g => new GroupData()
                    {
                        GroupId = g.GroupId,
                        GroupName = g.GroupName
                    })
                    .ToList();

                return Ok(new ApiResponse<List<GroupData>>(groups));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Изменение пользователя по ID.
        /// </summary>
        [HttpPut("UpdateUser/{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UserData userData)
        {
            try
            {
                // Проверяем, существует ли пользователь с заданным ID
                var user = _context.Users.FirstOrDefault(u => u.UserId == id);
                if (user == null)
                {
                    return NotFound(new { message = $"Пользователь с ID {id} не найден." });
                }

                // Обновляем данные пользователя
                user.LastName = userData.LastName ?? user.LastName;
                user.Name = userData.Name ?? user.Name;
                user.Surname = userData.Surname ?? user.Surname;
                user.Login = userData.Login ?? user.Login;
                user.Password = userData.Password ?? user.Password;
                user.RoleId = userData.RoleId ?? user.RoleId;
                user.GroupId = userData.GroupId ?? user.GroupId;

                // Сохраняем изменения
                _context.SaveChanges();

                return Ok(new { message = "Данные пользователя успешно обновлены." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Произошла ошибка при обновлении пользователя.", error = ex.Message });
            }
        }
    }
}
