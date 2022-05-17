using System;
using System.Collections.Generic;
using System.Linq;
using AssistantApi.Data;
using AssistantApi.Data.Models;
using AssistantApi.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AssistantApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private DatabaseContext _context;
        
        public MainController (DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Авторизация
        /// </summary>
        [Route("[action]")]
        [HttpPost]
        public ActionResult Auth(UserData userData)
        {
            try
            {
                var result = _context.Users
                    .Where(x => x.Login == userData.Login && x.Password == userData.Password)
                    .Select(x => new UserData
                    {
                        UserID = x.UserId,
                        Login = x.Login,
                        Password = x.Password,
                        Name = x.Name
                    }).FirstOrDefault();
                
                if (result == null)
                {
                    throw new Exception("Login and/or password invalid");
                }
                
                return new ObjectResult(new ApiResponse<UserData>(result));
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Регистрация
        /// </summary>
        [Route("[action]")]
        [HttpPost]
        public ActionResult Reg(UserData userData)
        {
            try
            {
                if (_context.Users.FirstOrDefault(x => x.Login == userData.Login) != null)
                {
                    throw new Exception("That login was exist");
                }

                User user = new User
                {
                    Login = userData.Login,
                    Password = userData.Password,
                    Name = userData.Name
                };
                _context.Users.Add(user);
                _context.SaveChanges();

                var result = _context.Users.Where(x => x.UserId == user.UserId)
                    .Select(x => new UserData
                    {
                        UserID = x.UserId,
                        Login = x.Login,
                        Password = x.Password,
                        Name = x.Name
                    }).FirstOrDefault();
                
                return new ObjectResult(new ApiResponse<UserData>(result));
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Вывод истории
        /// </summary>
        [Route("[action]")]
        [HttpGet]
        public ActionResult History(int id)
        {
            try
            {
                var result = _context.Histories
                    .Where(x => x.UserId == id)
                    .Select(x => new HistoryData()
                {
                    HistoryID = x.HistoryId,
                    Query = x.Query,
                    Response = x.Response,
                    UserID = x.UserId,
                    Date = x.Date
                }).ToList();
                
                return new ObjectResult(new ApiResponse<IEnumerable<HistoryData>>(result));
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ApiResponseMessage(ex.Message));
            }
        }

        /// <summary>
        /// Добавление запроса и ответа в историю
        /// </summary>
        [Route("[action]")]
        [HttpPost]
        public ActionResult AddHistory(HistoryData historyData)
        {
            try
            {
                var history = new History()
                {
                    HistoryId = historyData.HistoryID,
                    Query = historyData.Query,
                    Response = historyData.Response,
                    UserId = historyData.UserID,
                    Date = historyData.Date
                };
                _context.Histories.Add(history);
                _context.SaveChanges();

                var result = _context.Histories.Where(x => x.HistoryId == history.HistoryId)
                    .Select(x => new HistoryData()
                    {
                        HistoryID = x.HistoryId,
                        Query = x.Query,
                        Response = x.Response,
                        UserID = x.UserId,
                        Date = x.Date
                    }).FirstOrDefault();
                
                return new ObjectResult(new ApiResponse<HistoryData>(result));
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ApiResponseMessage(ex.Message));
            }
        }
    }
}