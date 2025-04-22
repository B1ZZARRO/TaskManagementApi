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
    public class UserController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public UserController(DatabaseContext context)
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

                var userData = new FullUserData
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

                return Ok(new ApiResponse<FullUserData>(userData));
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
                
                var userData = new FullUserData
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
                
                return Ok(new ApiResponse<FullUserData>(userData));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Получение списка пользователей по номеру группы
        /// </summary>
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
                    .Select(u => new FullUserData
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

                return Ok(new ApiResponse<List<FullUserData>>(users));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Получение списка всех пользователей.
        /// </summary>
        [Route("GetAllUsers")]
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.UserGroup)
                    .Select(u => new FullUserData
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

                return Ok(new ApiResponse<List<FullUserData>>(users));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
            
        }

        /// <summary>
        /// Получение списка всех ролей.
        /// </summary>
        [Route("GetAllRoles")]
        [HttpGet]
        public IActionResult GetAllRoles()
        {
            try
            {
                var roles = _context.Roles
                    .Select(r => new FullRoleData
                    {
                        RoleId = r.RoleId,
                        RoleName = r.RoleName
                    })
                    .ToList();

                return Ok(new ApiResponse<List<FullRoleData>>(roles));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }

        /// <summary>
        /// Получение списка всех групп.
        /// </summary>
        [Route("GetAllGroups")]
        [HttpGet]
        public IActionResult GetAllGroups()
        {
            try
            {
                var groups = _context.UserGroups
                    .Select(g => new FullGroupData()
                    {
                        GroupId = g.GroupId,
                        GroupName = g.GroupName
                    })
                    .ToList();

                return Ok(new ApiResponse<List<FullGroupData>>(groups));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Изменение пользователя по ID.
        /// </summary>
        [Route("UpdateUser/{id}")]
        [HttpPut]
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
        
        /// <summary>
        /// Изменение роли
        /// </summary>
        [Route("update-role/{roleId}")]
        [HttpPut]
        public IActionResult UpdateRole(int roleId, [FromBody] RoleData model)
        {
            try
            {
                var role = _context.Roles.FirstOrDefault(r => r.RoleId == roleId);
                if (role == null)
                    return NotFound(new ApiResponseMessage("Роль не найдена"));

                role.RoleName = model.RoleName ?? role.RoleName;

                _context.SaveChanges();
                return Ok(new ApiResponseMessage("Роль успешно обновлена"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Изменение группы
        /// </summary>
        [Route("update-group/{groupId}")]
        [HttpPut]
        public IActionResult UpdateUserGroup(int groupId, [FromBody] GroupData model)
        {
            try
            {
                var group = _context.UserGroups.FirstOrDefault(g => g.GroupId == groupId);
                if (group == null)
                    return NotFound(new ApiResponseMessage("Группа не найдена"));

                group.GroupName = model.GroupName ?? group.GroupName;

                _context.SaveChanges();
                return Ok(new ApiResponseMessage("Группа успешно обновлена"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Добавление роли
        /// </summary>
        [Route("AddRole")]
        [HttpPost]
        public IActionResult AddRole([FromBody] RoleData model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.RoleName))
                    return BadRequest(new ApiResponseMessage("Название роли не может быть пустым"));

                var existingRole = _context.Roles.FirstOrDefault(r => r.RoleName == model.RoleName);
                if (existingRole != null)
                    return BadRequest(new ApiResponseMessage("Такая роль уже существует"));

                var role = new Role
                {
                    RoleName = model.RoleName
                };

                _context.Roles.Add(role);
                _context.SaveChanges();

                return Ok(new
                {
                    message = "Роль успешно добавлена",
                    roleId = role.RoleId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Добавление группы
        /// </summary>
        [Route("AddUserGroup")]
        [HttpPost]
        public IActionResult AddUserGroup([FromBody] GroupData model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.GroupName))
                    return BadRequest(new ApiResponseMessage("Название группы не может быть пустым"));

                var existingGroup = _context.UserGroups.FirstOrDefault(g => g.GroupName == model.GroupName);
                if (existingGroup != null)
                    return BadRequest(new ApiResponseMessage("Такая группа уже существует"));

                var group = new UserGroup
                {
                    GroupName = model.GroupName
                };

                _context.UserGroups.Add(group);
                _context.SaveChanges();

                return Ok(new
                {
                    message = "Группа успешно добавлена",
                    groupId = group.GroupId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Удаление пользователя
        /// </summary>
        [Route("delete-user/{userId}")]
        [HttpDelete]
        public IActionResult DeleteUser(int userId)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                    return NotFound(new ApiResponseMessage("Пользователь не найден"));

                _context.Users.Remove(user);
                _context.SaveChanges();
                return Ok(new ApiResponseMessage("Пользователь успешно удалён"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Удаление роли
        /// </summary>
        [Route("delete-role/{roleId}")]
        [HttpDelete]
        public IActionResult DeleteRole(int roleId)
        {
            try
            {
                var role = _context.Roles.FirstOrDefault(r => r.RoleId == roleId);
                if (role == null)
                    return NotFound(new ApiResponseMessage("Роль не найдена"));

                _context.Roles.Remove(role);
                _context.SaveChanges();
                return Ok(new ApiResponseMessage("Роль успешно удалена"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Удаление группы
        /// </summary>
        [Route("delete-group/{groupId}")]
        [HttpDelete]
        public IActionResult DeleteUserGroup(int groupId)
        {
            try
            {
                var group = _context.UserGroups.FirstOrDefault(g => g.GroupId == groupId);
                if (group == null)
                    return NotFound(new ApiResponseMessage("Группа не найдена"));

                _context.UserGroups.Remove(group);
                _context.SaveChanges();
                return Ok(new ApiResponseMessage("Группа успешно удалена"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
    }
}